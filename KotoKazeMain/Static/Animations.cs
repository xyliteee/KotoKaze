using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using XyliteeeMainForm.Views;

namespace KotoKaze.Static
{
    internal class Animations
    {
        static public void PageSilderMoveing(Canvas canvas, int end)
        {
            DoubleAnimation animation = new()
            {
                To = end,
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            canvas.BeginAnimation(Canvas.TopProperty, animation);
        }
        static public void ButtonSilderMoveing(FrameworkElement widget, int end)
        {
            DoubleAnimation animation = new()
            {
                To = end,
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };
            widget.BeginAnimation(Canvas.LeftProperty, animation);
        }
        static public void ChangeOP(FrameworkElement widget, double? start,double? end,double time) 
        {
            if (GlobalData.AnimationLevel >0) time = 0;
            DoubleAnimation animation = new()
            {
                From = start,
                To = end,
                Duration = new Duration(TimeSpan.FromSeconds(time))
            };
            widget.BeginAnimation(UIElement.OpacityProperty, animation);

        }
        static public void ImageTurnRound(Image image, bool flag,double time = 1)
        {
            DoubleAnimation animation = new()
            {
                From = 0,
                To = -360,
                Duration = new Duration(TimeSpan.FromSeconds(time)),
            };
            RotateTransform rotateTransform = new(0);
            image.RenderTransform = rotateTransform;
            image.RenderTransformOrigin = new Point(0.5, 0.5);

            if (flag)
            {
                animation.RepeatBehavior = RepeatBehavior.Forever;
            }
            else
            {
                animation.RepeatBehavior = new RepeatBehavior(0);
            }

            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, animation);
        }
        static public void FrameMoving(FrameworkElement widget, double? from) 
        {
            if (GlobalData.AnimationLevel <= 1) 
            {
                DoubleAnimation animation = new()
                {
                    From = from,
                    To = 0,
                    Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                };
                widget.BeginAnimation(Canvas.TopProperty, animation);
            }
        }
        static public void ChangeSize(FrameworkElement widget, double time, double? from, double? to)
        {
            if (GlobalData.AnimationLevel <= 1)
            {
                ScaleTransform scale = new();
                widget.RenderTransform = scale;
                widget.RenderTransformOrigin = new(0.5, 0.5);
                DoubleAnimation animation = new()
                {
                    From = from,
                    To = to,
                    Duration = TimeSpan.FromSeconds(time)
                };
                scale.BeginAnimation(ScaleTransform.ScaleXProperty, animation);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, animation);
            }
            else 
            {
                ScaleTransform scale = new()
                {
                    ScaleX = (double)to!,
                    ScaleY = (double)to!
                };
                widget.RenderTransform = scale;
                widget.RenderTransformOrigin = new(0.5, 0.5);
            }
        }
        static public void ChangeBlur(FrameworkElement widget, double time, double? from, double? to)
        {
            if (GlobalData.AnimationLevel <= 0)
            {
                BlurEffect blur = new();
                widget.Effect = blur;
                DoubleAnimation animation = new()
                {
                    From = from,
                    To = to,
                    Duration = TimeSpan.FromSeconds(time)
                };
                blur.BeginAnimation(BlurEffect.RadiusProperty, animation);
            }
        }
    }
}
