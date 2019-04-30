// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Threading;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace osu.Framework.Testing.Drawables.Sections
{
    public class ToolbarRecordSection : ToolbarSection
    {
        private Button recordButton;
        private FillFlowContainer playbackControls;
        private TestBrowser browser;

        public ToolbarRecordSection()
        {
            AutoSizeAxes = Axes.X;
        }

        [BackgroundDependencyLoader]
        private void load(TestBrowser browser)
        {
            this.browser = browser;
            SpriteText maxFrameCount, currentFrame;

            InternalChild = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Y,
                AutoSizeAxes = Axes.X,
                Direction = FillDirection.Horizontal,
                Spacing = new Vector2(10),
                Children = new Drawable[]
                {
                    playbackControls = new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.X,
                        RelativeSizeAxes = Axes.Y,
                        Spacing = new Vector2(5),
                        Direction = FillDirection.Horizontal,
                        Children = new Drawable[]
                        {
                            new SpriteIcon
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Icon = FontAwesome.Solid.Circle,
                                Colour = Color4.Red,
                                Size = new Vector2(20),
                            },
                            currentFrame = new SpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Colour = FrameworkColour.Yellow,
                                Text = "0",
                                Font = new FontUsage("Roboto", weight: "Regular", fixedWidth: true)
                            },
                            new BasicSliderBar<int>
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Height = 20,
                                Width = 250,
                                Current = browser.CurrentFrame,
                                BackgroundColour = FrameworkColour.Blue,
                            },
                            maxFrameCount = new SpriteText
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                Text = "0",
                                Font = new FontUsage("Roboto", weight: "Regular", fixedWidth: true)
                            },
                        }
                    },
                    recordButton = new RecordButton
                    {
                        RelativeSizeAxes = Axes.Y,
                        Width = 100,
                        Action = changeState,
                    },
                }
            };

            browser.RecordState.BindValueChanged(updateState, true);
            browser.CurrentFrame.ValueChanged += frame => currentFrame.Text = frame.NewValue.ToString("00000");
            browser.CurrentFrame.MaxValueChanged += maxVal => maxFrameCount.Text = maxVal.ToString("00000");
        }

        private void changeState()
        {
            if (browser.RecordState.Value == RecordState.Stopped)
                browser.RecordState.Value = RecordState.Normal;
            else
                browser.RecordState.Value = browser.RecordState.Value + 1;
        }

        private void updateState(ValueChangedEvent<RecordState> args)
        {
            switch (args.NewValue)
            {
                case RecordState.Normal:
                    recordButton.Text = "record";
                    playbackControls.Hide();
                    break;
                case RecordState.Recording:
                    recordButton.Text = "stop";
                    playbackControls.Hide();
                    break;
                case RecordState.Stopped:
                    recordButton.Text = "reset";
                    playbackControls.Show();
                    break;
            }
        }

        private class RecordButton : Button
        {
            public RecordButton()
            {
                BackgroundColour = FrameworkColour.BlueGreen;
            }

            protected override SpriteText CreateText() => new SpriteText
            {
                Colour = FrameworkColour.Yellow,
                Font = new FontUsage("Roboto", weight: "Regular"),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            };
        }
    }
}
