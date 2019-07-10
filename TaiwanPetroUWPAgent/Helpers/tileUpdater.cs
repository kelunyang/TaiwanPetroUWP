using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace TaiwanPetroUWPAgent.Helpers
{
    public sealed class tileUpdater
    {
        public static async void update(int itemid, string itemname, string itemprice, string itemimg, bool type)
        {
            var tileContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    Branding = TileBranding.Name,
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            TextStacking = TileTextStacking.Center,
                            Children =
                {
                    new AdaptiveText()
                    {
                        Text = itemname,
                        HintStyle = AdaptiveTextStyle.Base,
                        HintAlign = AdaptiveTextAlign.Center
                    },
                    new AdaptiveText()
                    {
                        Text = itemprice,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle,
                        HintWrap = true
                    }
                },
                            PeekImage = new TilePeekImage()
                            {
                                Source = itemimg
                            }
                        }
                    },
                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                HintWeight = 33,
                                Children =
                                {
                                    new AdaptiveImage()
                                    {
                                        Source = itemimg
                                    }
                                }
                            },
                            new AdaptiveSubgroup()
                            {
                                Children =
                                {
                                    new AdaptiveText()
                                    {
                                        Text = itemname,
                                        HintStyle = AdaptiveTextStyle.Base
                                    },
                                    new AdaptiveText()
                                    {
                                        Text = itemprice,
                                        HintStyle = AdaptiveTextStyle.CaptionSubtle,
                                        HintWrap = true,
                                        HintMaxLines = 3
                                    }
                                },
                                HintTextStacking = AdaptiveSubgroupTextStacking.Center
                            }
                        }
                    }
                }
                        }
                    },
                    TileLarge = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            TextStacking = TileTextStacking.Center,
                            Children =
                {
                    new AdaptiveGroup()
                    {
                        Children =
                        {
                            new AdaptiveSubgroup()
                            {
                                HintWeight = 1
                            },
                            new AdaptiveSubgroup()
                            {
                                HintWeight = 2,
                                Children =
                                {
                                    new AdaptiveImage()
                                    {
                                        Source = itemimg
                                    }
                                }
                            },
                            new AdaptiveSubgroup()
                            {
                                HintWeight = 1
                            }
                        }
                    },
                    new AdaptiveText()
                    {
                        Text = itemname,
                        HintStyle = AdaptiveTextStyle.Base,
                        HintAlign = AdaptiveTextAlign.Center
                    },
                    new AdaptiveText()
                    {
                        Text = itemprice,
                        HintStyle = AdaptiveTextStyle.CaptionSubtle,
                        HintWrap = true,
                        HintMaxLines = 3,
                        HintAlign = AdaptiveTextAlign.Center
                    }
                }
                        }
                    }
                }
            };

            // Create the tile notification
            var tileNotif = new TileNotification(tileContent.GetXml());

            // And send the notification to the primary tile
            if (type)   //true means update primary tile
            {
                TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotif);
            }
            else
            {
                if (!SecondaryTile.Exists(itemid.ToString()))
                {
                    SecondaryTile sectile = new SecondaryTile(itemid.ToString(),
                                                    "台灣油價查詢",
                                                    "/",
                                                    new Uri("ms-appx:///Assets/tileIcon.png"),
                                                    TileSize.Square150x150);
                    await sectile.RequestCreateAsync();
                }
                // Get its updater
                var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(itemid.ToString());

                // And send the notification
                updater.Update(tileNotif);
            }
        }
    }
}
