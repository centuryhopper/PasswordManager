using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Graphics;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using PasswordManagerMobileApp.Services;
using CommunityToolkit.Maui.Markup;
using PasswordManagerMobileApp.Models;
using System.Windows.Input;


namespace PasswordManagerMobileApp.MVVM;

public partial class PasswordsPage : ContentPage
{
    private View BuildContent(PasswordsGridVM vm)
    {
        return new ScrollView
        {
            Orientation = ScrollOrientation.Vertical,
            Content = new ScrollView
            {
                Orientation = ScrollOrientation.Horizontal,
                Content = new Frame
                {
                    BorderColor = Colors.Gray,
                    CornerRadius = 8,
                    Padding = 10,
                    HasShadow = false,
                    BackgroundColor = Colors.White,
                    Content = new StackLayout
                    {
                        WidthRequest = 800,
                        Children =
                        {
                            // Table Header
                            new Grid
                            {
                                ColumnSpacing = 0,
                                RowSpacing = 0,
                                Margin = new Thickness(0, 0, 0, 1),
                                ColumnDefinitions = {
                                    new ColumnDefinition { Width = GridLength.Star },
                                    new ColumnDefinition { Width = GridLength.Star },
                                    new ColumnDefinition { Width = GridLength.Star }
                                },
                                Children =
                                {
                                    new Border
                                    {
                                        Stroke = Colors.Black,
                                        StrokeThickness = 1,
                                        Content = new Label
                                        {
                                            Text = "Title",
                                            TextColor = Colors.Black,
                                            FontAttributes = FontAttributes.Bold,
                                            Padding = 10
                                        }
                                    }.Column(0),

                                    new Border
                                    {
                                        Stroke = Colors.Black,
                                        StrokeThickness = 1,
                                        Content = new Label
                                        {
                                            Text = "Username",
                                            TextColor = Colors.Black,
                                            FontAttributes = FontAttributes.Bold,
                                            Padding = 10
                                        }
                                    }.Column(1),

                                    new Border
                                    {
                                        Stroke = Colors.Black,
                                        StrokeThickness = 1,
                                        Content = new Label
                                        {
                                            Text = "Password",
                                            TextColor = Colors.Black,
                                            FontAttributes = FontAttributes.Bold,
                                            Padding = 10
                                        }
                                    }.Column(2)
                                }
                            },

                            // Table Content
                            new CollectionView
                            {
                                ItemTemplate = new DataTemplate(() =>
                                {
                                    return new Grid
                                    {
                                        ColumnSpacing = 0,
                                        RowSpacing = 0,
                                        Margin = new Thickness(0, 0, 0, 1),
                                        ColumnDefinitions = {
                                            new ColumnDefinition { Width = GridLength.Star },
                                            new ColumnDefinition { Width = GridLength.Star },
                                            new ColumnDefinition { Width = GridLength.Star }
                                        },
                                        Children =
                                        {
                                            new Border
                                            {
                                                Stroke = Colors.Gray,
                                                StrokeThickness = 1,
                                                Content = new Label
                                                {
                                                    TextColor = Colors.Black,
                                                    Padding = 10
                                                }.Bind(Label.TextProperty, nameof(PasswordAccountDTO.Title))
                                            }.Column(0),

                                            new Border
                                            {
                                                Stroke = Colors.Gray,
                                                StrokeThickness = 1,
                                                Content = new Label
                                                {
                                                    TextColor = Colors.Black,
                                                    Padding = 10
                                                }.Bind(Label.TextProperty, nameof(PasswordAccountDTO.Username))
                                            }.Column(1),

                                            new Border
                                            {
                                                Stroke = Colors.Gray,
                                                StrokeThickness = 1,
                                                Content = new Grid
                                                {
                                                    ColumnDefinitions =
                                                    {
                                                        new ColumnDefinition { Width = GridLength.Star },
                                                        new ColumnDefinition { Width = GridLength.Auto }
                                                    },
                                                    Children =
                                                    {
                                                        new Label
                                                        {
                                                            TextColor = Colors.Black,
                                                            Padding = 10
                                                        }
                                                        .Bind(Label.TextProperty, nameof(PasswordAccountDTO.DisplayedPassword))
                                                        .Column(0),

                                                        new Button
                                                        {
                                                            // Text = "👁",
                                                            FontSize = 14,
                                                            Padding = new Thickness(5, 0),
                                                            BackgroundColor = Colors.Transparent
                                                        }
                                                        .Bind(Button.TextProperty, nameof(PasswordAccountDTO.EyeIcon))
                                                        .Bind(Button.CommandProperty, nameof(PasswordAccountDTO.TogglePasswordVisibilityCommand))
                                                        .Column(1)
                                                    }
                                                }
                                            }.Column(2)

                                        }
                                    };
                                })
                            }.Bind(ItemsView.ItemsSourceProperty, nameof(vm.PagedItems)),

                            // Pagination Controls
                            new Grid
                            {
                                ColumnDefinitions = {
                                    new ColumnDefinition { Width = GridLength.Auto },
                                    new ColumnDefinition { Width = GridLength.Star },
                                    new ColumnDefinition { Width = GridLength.Auto }
                                },
                                Margin = new Thickness(0, 20, 0, 0),
                                Children =
                                {
                                    new Button
                                    {
                                        Text = "◀ Previous"
                                    }
                                    .Bind(Button.CommandProperty, "PreviousPageCommand")
                                    .Bind(IsEnabledProperty, "CanGoPrevious")
                                    .Column(0),

                                    new Label()
                                    .Center()
                                    .Bind(Label.TextProperty, "CurrentPage")
                                    .Column(1),

                                    new Button
                                    {
                                        Text = "Next ▶"
                                    }
                                    .Bind(Button.CommandProperty, "NextPageCommand")
                                    .Bind(IsEnabledProperty, "CanGoNext")
                                    .Column(2)
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
