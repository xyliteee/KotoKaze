using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
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

        static public void ImageTurnRound(Image image, bool flag)
        {
            DoubleAnimation animation = new()
            {
                From = 0,
                To = -360,
                Duration = new Duration(TimeSpan.FromSeconds(1)),
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
    }
}
