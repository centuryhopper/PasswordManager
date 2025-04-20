using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Graphics;
using CommunityToolkit.Maui.Markup;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;
using PasswordManagerMobileApp.Services;
using PasswordManagerMobileApp.Models;


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
                                                    Padding = 10
                                                }.Bind(Label.TextProperty, nameof(PasswordAccountDTO.Title))
                                            }.Column(0),

                                            new Border
                                            {
                                                Stroke = Colors.Gray,
                                                StrokeThickness = 1,
                                                Content = new Label
                                                {
                                                    Padding = 10
                                                }.Bind(Label.TextProperty, nameof(PasswordAccountDTO.Username))
                                            }.Column(1),

                                            new Border
                                            {
                                                Stroke = Colors.Gray,
                                                StrokeThickness = 1,
                                                Content = new Label
                                                {
                                                    Padding = 10
                                                }.Bind(Label.TextProperty, nameof(PasswordAccountDTO.Password))
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

                                    new Label
                                    {
                                        HorizontalOptions = LayoutOptions.Center,
                                        VerticalOptions = LayoutOptions.Center
                                    }
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
