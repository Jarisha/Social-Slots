#ifndef _TRAMPOLINE_IPHONE_VIEW_H_
#define _TRAMPOLINE_IPHONE_VIEW_H_

#import <UIKit/UIKit.h>


@interface EAGLView : UIView {}
@end

@interface UnityViewController : UIViewController {}
@end


UIViewController*   UnityGetGLViewController();
UIView*             UnityGetGLView();
UIWindow*           UnityGetMainWindow();

void    CreateViewHierarchy();
void    ReleaseViewHierarchy();

void    OnUnityStartLoading();
void    OnUnityReady();

void    CheckOrientationRequest();
void    OrientTo(int requestedOrient);

#endif // _TRAMPOLINE_IPHONE_VIEW_H_
