using Razorwing.Framework.Graphics.Transforms;
using Razorwing.Framework.Threading;
using System;
using Razorwing.Framework.Configuration;
using Razorwing.Framework.Utils;
using Microsoft.Xna.Framework;

namespace Razorwing.Framework.Graphics
{
    public static class TransformSequenceExtensions
    {
        //public static TransformSequence<T> Expire<T>(this TransformSequence<T> t)
        //    where T : Drawable =>
        //    t.Append(o => o.Expire());

        public static TransformSequence<T> Schedule<T>(this TransformSequence<T> t, Action scheduledAction)
            where T : Drawable =>
            t.Append(o => o.Schedule(scheduledAction));

        public static TransformSequence<T> Schedule<T>(this TransformSequence<T> t, Action scheduledAction, out ScheduledDelegate scheduledDelegate)
            where T : Drawable =>
            t.Append(o => o.Schedule(scheduledAction), out scheduledDelegate);

        public static TransformSequence<T> Spin<T>(this TransformSequence<T> t, double revolutionDuration, RotationDirection direction, float startRotation = 0)
            where T : Drawable =>
            t.Loop(d => d.RotateTo(startRotation).RotateTo(startRotation + (direction == RotationDirection.Clockwise ? 360 : -360), revolutionDuration));

        public static TransformSequence<T> Spin<T>(this TransformSequence<T> t, double revolutionDuration, RotationDirection direction, float startRotation, int numRevolutions)
            where T : Drawable =>
            t.Loop(0, numRevolutions, d => d.RotateTo(startRotation).RotateTo(startRotation + (direction == RotationDirection.Clockwise ? 360 : -360), revolutionDuration));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.Alpha"/> to 1 over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> FadeIn<T>(this TransformSequence<T> t, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.FadeIn(duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.Alpha"/> from 0 to 1 over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> FadeInFromZero<T>(this TransformSequence<T> t, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.FadeInFromZero(duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.Alpha"/> to 0 over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> FadeOut<T>(this TransformSequence<T> t, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.FadeOut(duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.Alpha"/> from 1 to 0 over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> FadeOutFromOne<T>(this TransformSequence<T> t, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.FadeOutFromOne(duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.Alpha"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> FadeTo<T>(this TransformSequence<T> t, float newAlpha, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.FadeTo(newAlpha, duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.Colour"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> FadeColour<T>(this TransformSequence<T> t, Color newColour, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.FadeColour(newColour, duration, easing));

        /// <summary>
        /// Instantaneously flashes <see cref="Drawable.Colour"/>, then smoothly changes it back over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> FlashColour<T>(this TransformSequence<T> t, Color flashColour, double duration, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.FlashColour(flashColour, duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.Rotation"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> RotateTo<T>(this TransformSequence<T> t, float newRotation, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.RotateTo(newRotation, duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.Scale"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> ScaleTo<T>(this TransformSequence<T> t, float newScale, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.ScaleTo(newScale, duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.Scale"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> ScaleTo<T>(this TransformSequence<T> t, Vector2 newScale, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.ScaleTo(newScale, duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.Size"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> ResizeTo<T>(this TransformSequence<T> t, float newSize, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.ResizeTo(newSize, duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.Size"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> ResizeTo<T>(this TransformSequence<T> t, Vector2 newSize, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.ResizeTo(newSize, duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.Width"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> ResizeWidthTo<T>(this TransformSequence<T> t, float newWidth, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.ResizeWidthTo(newWidth, duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.Height"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> ResizeHeightTo<T>(this TransformSequence<T> t, float newHeight, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.ResizeHeightTo(newHeight, duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.Position"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> MoveTo<T>(this TransformSequence<T> t, Vector2 newPosition, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.MoveTo(newPosition, duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.X"/> or <see cref="Drawable.Y"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> MoveTo<T>(this TransformSequence<T> t, Direction direction, float destination, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.MoveTo(direction, destination, duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.X"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> MoveToX<T>(this TransformSequence<T> t, float destination, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.MoveToX(destination, duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.Y"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> MoveToY<T>(this TransformSequence<T> t, float destination, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.MoveToY(destination, duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.Position"/> by an offset to its final value over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> MoveToOffset<T>(this TransformSequence<T> t, Vector2 offset, double duration = 0, Easing easing = Easing.None)
            where T : Drawable =>
            t.Append(o => o.MoveToOffset(offset, duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.RelativePosition"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> MoveToRelative<T>(this TransformSequence<T> t, Vector2 destination, double duration = 0, Easing easing = Easing.None) where T : Drawable =>
            t.Append(o => o.MoveToRelative(destination, duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.VAlign"/> and <see cref="Drawable.HAlign"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> MoveAlign<T>(this TransformSequence<T> t, Vector2 destination, double duration = 0, Easing easing = Easing.None) where T : Drawable =>
            t.Append(o => o.MoveAlign(destination, duration, easing));


        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.VAlign"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> MoveVAlign<T>(this TransformSequence<T> t, float destination, double duration = 0, Easing easing = Easing.None) where T : Drawable =>
            t.Append(o => o.MoveVAlign(destination, duration, easing));

        /// <summary>
        /// Smoothly adjusts <see cref="Drawable.HAlign"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> MoveHAlign<T>(this TransformSequence<T> t, float destination, double duration = 0, Easing easing = Easing.None) where T : Drawable =>
            t.Append(o => o.MoveHAlign(destination, duration, easing));

        /// <summary>
        /// Smoothly adjusts the value of a <see cref="Bindable{TValue}"/> over time.
        /// </summary>
        /// <returns>A <see cref="TransformSequence{T}"/> to which further transforms can be added.</returns>
        public static TransformSequence<T> TransformBindableTo<T, TValue>(this TransformSequence<T> t, Bindable<TValue> bindable, TValue newValue, double duration = 0, Easing easing = Easing.None,
                                                                          InterpolationFunc<TValue> interpolationFunc = null)
            where T : ITransformable =>
            t.Append(o => o.TransformBindableTo(bindable, newValue, duration, easing, interpolationFunc));
    }
}
