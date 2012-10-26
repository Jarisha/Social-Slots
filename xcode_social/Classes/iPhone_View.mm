
#include "iPhone_View.h"
#include "iPhone_Common.h"

#import <QuartzCore/QuartzCore.h>
#import <UIKit/UIApplication.h>


enum EnabledOrientation
{
    kAutorotateToPortrait = 1,
    kAutorotateToPortraitUpsideDown = 2,
    kAutorotateToLandscapeLeft = 4,
    kAutorotateToLandscapeRight = 8
};


enum ScreenOrientation
{
    kScreenOrientationUnknown,
    portrait,
    portraitUpsideDown,
    landscapeLeft,
    landscapeRight,
    autorotation,
    kScreenOrientationCount
};


static ScreenOrientation _curOrientation = portrait;

extern "C" __attribute__((visibility ("default"))) NSString * const kUnityViewWillRotate = @"kUnityViewWillRotate";
extern "C" __attribute__((visibility ("default"))) NSString * const kUnityViewDidRotate = @"kUnityViewDidRotate";

struct EAGLSurfaceDesc;
extern EAGLSurfaceDesc _surface;
void RecreateSurface(EAGLSurfaceDesc* surface, bool insideRepaint);


ScreenOrientation ConvertToUnityScreenOrientation(int hwOrient, EnabledOrientation* outAutorotOrient);
ScreenOrientation UnityRequestedScreenOrientation();

bool UnityIsOrientationEnabled(EnabledOrientation orientation);
void UnitySetScreenOrientation(ScreenOrientation orientation);
bool UnityUseAnimatedAutorotation();
void UnityKeyboardOrientationStep1();
void UnityKeyboardOrientationStep2();
int  UnityGetShowActivityIndicatorOnLoading();

void UnitySendTouchesBegin(NSSet* touches, UIEvent* event);
void UnitySendTouchesEnded(NSSet* touches, UIEvent* event);
void UnitySendTouchesCancelled(NSSet* touches, UIEvent* event);
void UnitySendTouchesMoved(NSSet* touches, UIEvent* event);

void UnityFinishRendering();

static UIWindow*                _window             = nil;
static UnityViewController*     _viewController     = nil;
static EAGLView*                _view               = nil;
static UIImageView*             _splashView         = nil;
static UIActivityIndicatorView* _activityIndicator  = nil;

bool _shouldAttemptReorientation = false;

UIWindow*           UnityGetMainWindow()        { return _window; }
UIViewController*   UnityGetGLViewController()  { return _viewController; }
UIView*             UnityGetGLView()            { return _view; }

static CGAffineTransform TransformForOrientation( ScreenOrientation orient )
{
    switch(orient)
    {
        case portrait:              return CGAffineTransformIdentity;
        case portraitUpsideDown:    return CGAffineTransformMakeRotation(M_PI);
        case landscapeLeft:         return CGAffineTransformMakeRotation(M_PI_2);
        case landscapeRight:        return CGAffineTransformMakeRotation(-M_PI_2);

        default:                    return CGAffineTransformIdentity;
    }
    return CGAffineTransformIdentity;
}

static CGRect ContentRectForOrientation( ScreenOrientation orient )
{
    CGRect screenRect = [[UIScreen mainScreen] bounds];

    switch(orient)
    {
        case portrait:
        case portraitUpsideDown:
            return screenRect;
        case landscapeLeft:
        case landscapeRight:
            return CGRectMake(screenRect.origin.y, screenRect.origin.x, screenRect.size.height, screenRect.size.width);
        default:
            return screenRect;
    }

    return screenRect;
}

static UIInterfaceOrientation ConvertToIosScreenOrientation(ScreenOrientation orient)
{
    switch( orient )
    {
        case portrait:              return UIInterfaceOrientationPortrait;
        case portraitUpsideDown:    return UIInterfaceOrientationPortraitUpsideDown;
        // landscape left/right have switched values in device/screen orientation
        // though unity docs are adjusted with device orientation values, so swap here
        case landscapeLeft:         return UIInterfaceOrientationLandscapeRight;
        case landscapeRight:        return UIInterfaceOrientationLandscapeLeft;

        default:                    return UIInterfaceOrientationPortrait;
    }

    return UIInterfaceOrientationPortrait;
}


static NSString* SplashViewImage(UIInterfaceOrientation orient)
{
    bool need2xSplash = false;
#ifdef __IPHONE_4_0
    if ( [[UIScreen mainScreen] respondsToSelector:@selector(scale)] && [UIScreen mainScreen].scale > 1.0 )
        need2xSplash = true;
#endif

    bool needOrientedSplash = false;
    bool needPortraitSplash = true;

    if (UI_USER_INTERFACE_IDIOM() != UIUserInterfaceIdiomPhone)
    {
        bool devicePortrait  = UIDeviceOrientationIsPortrait(orient);
        bool deviceLandscape = UIDeviceOrientationIsLandscape(orient);

        NSArray* supportedOrientation = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"UISupportedInterfaceOrientations"];
        bool rotateToPortrait  =   [supportedOrientation containsObject: @"UIInterfaceOrientationPortrait"]
                                || [supportedOrientation containsObject: @"UIInterfaceOrientationPortraitUpsideDown"];
        bool rotateToLandscape =   [supportedOrientation containsObject: @"UIInterfaceOrientationLandscapeLeft"]
                                || [supportedOrientation containsObject: @"UIInterfaceOrientationLandscapeRight"];


        needOrientedSplash = true;
        if (devicePortrait && rotateToPortrait)
            needPortraitSplash = true;
        else if (deviceLandscape && rotateToLandscape)
            needPortraitSplash = false;
        else if (rotateToPortrait)
            needPortraitSplash = true;
        else
            needPortraitSplash = false;
    }

    const char* portraitSuffix  = needOrientedSplash ? "-Portrait" : "";
    const char* landscapeSuffix = needOrientedSplash ? "-Landscape" : "";

    const char* szSuffix        = need2xSplash ? "@2x" : "";
    const char* orientSuffix    = needPortraitSplash ? portraitSuffix : landscapeSuffix;

    return [NSString stringWithFormat:@"Default%s%s", orientSuffix, szSuffix];
}

static void CreateActivityIndicator()
{
    const int activityIndicatorStyle = UnityGetShowActivityIndicatorOnLoading();
    if( activityIndicatorStyle >= 0)
        _activityIndicator = [[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:(UIActivityIndicatorViewStyle)activityIndicatorStyle];
}

void UnityStartActivityIndicator()
{
    if(_activityIndicator == nil)
    {
        CreateActivityIndicator();
        [_view addSubview: _activityIndicator];
    }

    if( _activityIndicator )
    {
        _activityIndicator.center = CGPointMake([_view bounds].size.width/2, [_view bounds].size.height/2);
        [_activityIndicator startAnimating];
    }
}

void UnityStopActivityIndicator()
{
    if( _activityIndicator )
    {
        [_activityIndicator stopAnimating];
        [_activityIndicator removeFromSuperview];
        [_activityIndicator release];
        _activityIndicator = nil;
    }
}

void CreateViewHierarchy()
{
    CGRect screenRect = [[UIScreen mainScreen] bounds];

    _window         = [[UIWindow alloc] initWithFrame: screenRect];
    _view           = [[EAGLView alloc] initWithFrame: screenRect];
    _viewController = [[UnityViewController alloc] init];
    _splashView     = [[UIImageView alloc] initWithFrame: screenRect];

    if( _ios30orNewer )
        _viewController.wantsFullScreenLayout = TRUE;

    _viewController.view = _view;

    _splashView.image = [UIImage imageNamed:SplashViewImage(UIInterfaceOrientationPortrait)];
    if (UI_USER_INTERFACE_IDIOM() != UIUserInterfaceIdiomPhone)
    {
        _splashView.autoresizingMask = UIViewAutoresizingFlexibleWidth | UIViewAutoresizingFlexibleHeight;
        _splashView.autoresizesSubviews = YES;
    }

#ifdef __IPHONE_4_0
    if(   [_splashView respondsToSelector:@selector(setContentScaleFactor:)]
       && [[UIScreen mainScreen] respondsToSelector:@selector(scale)]
      )
        [ _splashView setContentScaleFactor: [UIScreen mainScreen].scale ];
#endif

    CreateActivityIndicator();
    [_splashView addSubview: _activityIndicator];

    [_view addSubview: _splashView];

    {
        NSString* initialOrientation = [[[NSBundle mainBundle] infoDictionary] objectForKey:@"UIInterfaceOrientation"];
        if( [initialOrientation isEqualToString: @"UIInterfaceOrientationPortrait"] == YES )
            _curOrientation = portrait;
        else if( [initialOrientation isEqualToString: @"UIInterfaceOrientationPortraitUpsideDown"] == YES )
            _curOrientation = portraitUpsideDown;
        else if( [initialOrientation isEqualToString: @"UIInterfaceOrientationLandscapeLeft"] == YES )
            _curOrientation = landscapeLeft;
        else if( [initialOrientation isEqualToString: @"UIInterfaceOrientationLandscapeRight"] == YES )
            _curOrientation = landscapeRight;
        UnitySetScreenOrientation(_curOrientation);
    }

    // add only now so controller have chance to reorient *all* added views
    [_window addSubview: _view];

    if( [_window respondsToSelector:@selector(rootViewController)] )
        _window.rootViewController = _viewController;

    [_window bringSubviewToFront: _splashView];
}

void ReleaseViewHierarchy()
{
    if(_activityIndicator)
    {
        [_activityIndicator release];
        _activityIndicator = nil;
    }

    if(_splashView)
    {
        [_splashView release];
        _splashView = nil;
    }

    [_viewController release];
    _viewController = nil;

    [_view release];
    _view = nil;

    [_window release];
    _window = nil;
}

void OnUnityStartLoading()
{
    UnityStartActivityIndicator();
}

void OnUnityReady()
{
    UnityStopActivityIndicator();

    [_splashView removeFromSuperview];
    [_splashView release];
    _splashView = nil;
}

void NotifyAutoOrientationChange()
{
    _shouldAttemptReorientation = true;
}

static bool OrientationWillChangeSurfaceExtents( ScreenOrientation prevOrient, ScreenOrientation targetOrient )
{
    bool prevLandscape   = ( prevOrient == landscapeLeft || prevOrient == landscapeRight );
    bool targetLandscape = ( targetOrient == landscapeLeft || targetOrient == landscapeRight );

    return( prevLandscape != targetLandscape );
}

void OrientTo(int requestedOrient_)
{
    ScreenOrientation requestedOrient = (ScreenOrientation)requestedOrient_;

    UnityFinishRendering();
    [CATransaction begin];
    {
        UnityKeyboardOrientationStep1();
        _view.transform  = TransformForOrientation(requestedOrient);
        _view.bounds     = ContentRectForOrientation(requestedOrient);

        [UIApplication sharedApplication].statusBarOrientation = ConvertToIosScreenOrientation(requestedOrient);

        UnitySetScreenOrientation(requestedOrient);
        if( OrientationWillChangeSurfaceExtents(_curOrientation, requestedOrient) )
            RecreateSurface(&_surface, false);
    }
    [CATransaction commit];

    [CATransaction begin];
    UnityKeyboardOrientationStep2();
    [CATransaction commit];

    _curOrientation = requestedOrient;
}

void CheckOrientationRequest()
{
    ScreenOrientation requestedOrient = UnityRequestedScreenOrientation();
    if(requestedOrient == autorotation)
    {
    #ifdef __IPHONE_5_0
        if(_ios50orNewer && _shouldAttemptReorientation)
            [UIViewController attemptRotationToDeviceOrientation];
    #endif
        _shouldAttemptReorientation = false;
    }
    else if(requestedOrient != _curOrientation)
    {
        OrientTo(requestedOrient);
    }
}

@implementation UnityViewController
-(BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    EnabledOrientation targetAutorot   = kAutorotateToPortrait;
    ScreenOrientation  targetOrient    = ConvertToUnityScreenOrientation(interfaceOrientation, &targetAutorot);
    ScreenOrientation  requestedOrientation = UnityRequestedScreenOrientation();

    if (requestedOrientation != autorotation)
        return (requestedOrientation == targetOrient);

    return UnityIsOrientationEnabled(targetAutorot);
}

- (void)willRotateToInterfaceOrientation:(UIInterfaceOrientation)toInterfaceOrientation duration:(NSTimeInterval)duration
{
    _curOrientation = ConvertToUnityScreenOrientation(toInterfaceOrientation, 0);
    [[NSNotificationCenter defaultCenter] postNotificationName:kUnityViewWillRotate object:self];

    if(_splashView || !UnityUseAnimatedAutorotation())
        [UIView setAnimationsEnabled:NO];

    if(_splashView && UI_USER_INTERFACE_IDIOM() != UIUserInterfaceIdiomPhone)
        _splashView.image = [UIImage imageNamed:SplashViewImage(toInterfaceOrientation)];

}

- (void)didRotateFromInterfaceOrientation:(UIInterfaceOrientation)fromInterfaceOrientation
{
    if( _splashView )
    {
        if( UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPhone )
        {
            CGRect rect  = [[UIScreen mainScreen] bounds];

            // in case of landscape we want to rotate *back*
            if( _curOrientation == landscapeLeft || _curOrientation == landscapeRight )
            {
                _splashView.transform = TransformForOrientation(_curOrientation == landscapeRight ? landscapeLeft : landscapeRight);
                _splashView.center    = CGPointMake(rect.size.height/2, rect.size.width/2);
            }
            else
            {
                _splashView.transform = TransformForOrientation(_curOrientation);
                _splashView.center    = CGPointMake(rect.size.width/2, rect.size.height/2);
            }
            _splashView.bounds    = rect;
        }
    }

    if (_activityIndicator)
        _activityIndicator.center = CGPointMake([self.view bounds].size.width/2, [self.view bounds].size.height/2);

    UnitySetScreenOrientation(_curOrientation);
    if( OrientationWillChangeSurfaceExtents( ConvertToUnityScreenOrientation(fromInterfaceOrientation,0), _curOrientation ) )
        RecreateSurface(&_surface, false);

    if( _splashView || !UnityUseAnimatedAutorotation() )
        [UIView setAnimationsEnabled:YES];

    [[NSNotificationCenter defaultCenter] postNotificationName:kUnityViewDidRotate object:self];
}

@end

@implementation EAGLView

+ (Class) layerClass
{
    return [CAEAGLLayer class];
}

- (id) initWithFrame:(CGRect)frame
{
    if( (self = [super initWithFrame:frame]) )
    {
        [self setMultipleTouchEnabled:YES];
        [self setExclusiveTouch:YES];
    }
    return self;
}

- (void) touchesBegan:(NSSet*)touches withEvent:(UIEvent*)event
{
    UnitySendTouchesBegin(touches, event);
}
- (void) touchesEnded:(NSSet*)touches withEvent:(UIEvent*)event
{
    UnitySendTouchesEnded(touches, event);
}
- (void) touchesCancelled:(NSSet*)touches withEvent:(UIEvent*)event
{
    UnitySendTouchesCancelled(touches, event);
}
- (void) touchesMoved:(NSSet*)touches withEvent:(UIEvent*)event
{
    UnitySendTouchesMoved(touches, event);
}

@end
