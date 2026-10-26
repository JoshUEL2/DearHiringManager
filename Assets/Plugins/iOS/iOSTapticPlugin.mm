#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface iOSTapticBridge : NSObject
+ (void)triggerImpact:(int)style;
+ (void)triggerSelection;
+ (void)triggerNotification:(int)type;
@end

@implementation iOSTapticBridge

static UIImpactFeedbackGenerator *lightGen = nil;
static UIImpactFeedbackGenerator *mediumGen = nil;
static UIImpactFeedbackGenerator *heavyGen = nil;
static UISelectionFeedbackGenerator *selectionGen = nil;
static UINotificationFeedbackGenerator *notificationGen = nil;

+ (void)initializeGenerators {
    static dispatch_once_t onceToken;
    dispatch_once(&onceToken, ^{
        lightGen = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleLight];
        mediumGen = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleMedium];
        heavyGen = [[UIImpactFeedbackGenerator alloc] initWithStyle:UIImpactFeedbackStyleHeavy];
        selectionGen = [[UISelectionFeedbackGenerator alloc] init];
        notificationGen = [[UINotificationFeedbackGenerator alloc] init];
        
        [lightGen prepare];
        [mediumGen prepare];
        [heavyGen prepare];
        [selectionGen prepare];
        [notificationGen prepare];
    });
}

+ (void)triggerImpact:(int)style {
    [self initializeGenerators];
    dispatch_async(dispatch_get_main_queue(), ^{
        switch (style) {
            case 0: // Light
                [lightGen impactOccurred];
                [lightGen prepare];
                break;
            case 1: // Medium
                [mediumGen impactOccurred];
                [mediumGen prepare];
                break;
            case 2: // Heavy
                [heavyGen impactOccurred];
                [heavyGen prepare];
                break;
            default:
                [lightGen impactOccurred];
                [lightGen prepare];
                break;
        }
    });
}

+ (void)triggerSelection {
    [self initializeGenerators];
    dispatch_async(dispatch_get_main_queue(), ^{
        [selectionGen selectionChanged];
        [selectionGen prepare];
    });
}

+ (void)triggerNotification:(int)type {
    [self initializeGenerators];
    dispatch_async(dispatch_get_main_queue(), ^{
        switch (type) {
            case 0: // Success
                [notificationGen notificationOccurred:UINotificationFeedbackTypeSuccess];
                break;
            case 1: // Warning
                [notificationGen notificationOccurred:UINotificationFeedbackTypeWarning];
                break;
            case 2: // Error
                [notificationGen notificationOccurred:UINotificationFeedbackTypeError];
                break;
        }
        [notificationGen prepare];
    });
}

@end

extern "C" {
    void _iOS_TriggerImpact(int style) {
        [iOSTapticBridge triggerImpact:style];
    }

    void _iOS_TriggerSelection() {
        [iOSTapticBridge triggerSelection];
    }

    void _iOS_TriggerNotification(int type) {
        [iOSTapticBridge triggerNotification:type];
    }
}
