// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace osu.Framework.Tests.Visual.Audio
{
    public partial class TestSceneTrackPitch : FrameworkTestScene
    {
        private DrawableTrack drawableTrack = null!;
        private AudioContainer audioContainer = null!;
        private SliderBar<double> sliderBar = null!;

        private SpriteText text = null!;

        [BackgroundDependencyLoader]
        private void load(ITrackStore tracks)
        {
            Children = new Drawable[]
            {
                audioContainer = new AudioContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        sliderBar = new BasicSliderBar<double>
                        {
                            RelativeSizeAxes = Axes.Both,
                            Size = new Vector2(1, 0.1f),
                            Current = new BindableDouble
                            {
                                MinValue = -12 * 100,
                                MaxValue = 12 * 100,
                                Precision = 1
                            }
                        },
                        text = new SpriteText
                        {
                            Padding = new MarginPadding(10),
                            Text = $"cents: {sliderBar.Current.Value}, semitones: {sliderBar.Current.Value / 100}, octaves: {sliderBar.Current.Value / 1200:F0}",
                            Origin = Anchor.Centre,
                            Anchor = Anchor.Centre
                        },
                        drawableTrack = new DrawableTrack(tracks.Get("sample-track.mp3"))
                        {
                            Looping = true
                        },
                    }
                }
            };

            drawableTrack.Start();
            audioContainer.AddAdjustment(AdjustableProperty.Pitch, sliderBar.Current);
            sliderBar.Current.BindValueChanged(_ => text.Text = $"cents: {sliderBar.Current.Value}, semitones: {sliderBar.Current.Value / 100:F0}, octaves: {sliderBar.Current.Value / 1200:F0}", true);
        }
    }
}
