namespace BasketTracker.Mobile.Core.Baskets

open BasketTracker.Mobile.Core
open Xamarin.Forms
open System
open System.Collections
open System.ComponentModel

type BasketListPage(vm: ListPageViewModel, config: ListPageConfiguration, navigator: Navigator) as self =
    inherit ContentPage()

    let listView = new ListView(ItemTemplate = new DataTemplate(fun () -> box (new BasketViewCell(config.Cell, navigator))))
    
    let emptyMsg = new Label(Text = config.EmptyMessage, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center)

    let add = 
        new ToolbarItem(
            config.Add.Title, 
            config.Add.Icon, 
            fun () -> navigator.Basket.NavigateToAdd navigator <| Context self.BindingContext)

    let layout = 
        let layout = new StackLayout(Padding = new Thickness(config.Padding))
        layout.Children.Add(emptyMsg)
        layout.Children.Add(listView)
        layout

    do
        // Bindings
        self.SetBinding(ContentPage.TitleProperty, "Title")
        listView.SetBinding(ListView.ItemsSourceProperty, "List")
        emptyMsg.SetBinding(Label.IsVisibleProperty, "List", converter = new IsEmptyConverter())

        // Toolbar items
        self.ToolbarItems.Add(add)
        
        self.BindingContext <- vm
        self.Content <- layout

    override self.OnAppearing() =
        base.OnAppearing()
        vm.Refresh()


and BasketViewCell(config: CellConfiguration, navigator: Navigator) as self=
    inherit ViewCell()

    let image  = new Image()
    let date   = new Label(HorizontalTextAlignment = TextAlignment.Start, VerticalTextAlignment = TextAlignment.Center)
    let time   = new Label(HorizontalTextAlignment = TextAlignment.Start, VerticalTextAlignment = TextAlignment.Center)
    let amount = new Label(HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center)
    let update  = new MenuItem(Text = config.Edit.Title, Icon = FileImageSource.op_Implicit config.Edit.Icon)
    let remove  = new MenuItem(Text = config.Delete.Title, Icon = FileImageSource.op_Implicit config.Delete.Icon)

    let layout = 
        let layout = new Grid(Padding = new Thickness(config.Padding))
        layout.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(1., GridUnitType.Star)))
        layout.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(3., GridUnitType.Star)))
        layout.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(1., GridUnitType.Star)))
        layout.Children.Add(date, 0, 0)
        layout.Children.Add(time, 1, 0)
        layout.Children.Add(amount, 2, 0)
        layout

    do
        // Navigation events
        self.Tapped.Add(fun _ -> navigator.Item.NavigateToItemList navigator <| Context self.BindingContext)
        update.Clicked.Add(fun _ -> navigator.Basket.NavigateToUpdate navigator <| Context self.BindingContext)

        // Bindings
        image.SetBinding(Image.SourceProperty, "Image")
        date.SetBinding(Label.TextProperty, "Date", stringFormat = "{0:ddd d MMM}")
        time.SetBinding(Label.TextProperty, "Date", stringFormat = "{0:hh:mm tt}")
        amount.SetBinding(Label.TextProperty, "Total", stringFormat = "{0:C2}")
        remove.SetBinding(MenuItem.CommandProperty, "RemoveCommand")

        // Context actions
        self.ContextActions.Add(update)
        self.ContextActions.Add(remove)

        self.View <- layout


type AddBasketPage(vm) as self =
    inherit ContentPage()
    
    let date = new DatePicker(Format = "dd MMM yyyy")
    let time = new TimePicker()
    let save =
        new ToolbarItem(
            "Save this basket", 
            "save", 
            fun () -> 
                self.Navigation.PopAsync()
                |> Async.AwaitTask
                |> Async.Ignore
                |> Async.StartImmediate)

    let layout =
        let layout = new StackLayout(Padding = new Thickness(10.))
        layout.Children.Add(date)
        layout.Children.Add(time)
        layout

    do
        // Bindings
        date.SetBinding(DatePicker.DateProperty, "Date")            
        time.SetBinding(TimePicker.TimeProperty, "Time")
        self.SetBinding(ContentPage.TitleProperty, "Title") 
        save.SetBinding(ToolbarItem.CommandProperty, "AddCommand")

        // Toolbar items
        base.ToolbarItems.Add(save)

        base.BindingContext <- vm
        base.Content <- layout


type UpdateBasketPage(vm) as self =
    inherit ContentPage()
    
    let date = new DatePicker(Format = "dd MMM yyyy")
    let time = new TimePicker()
    let save =
        new ToolbarItem(
            "Update this basket", 
            "save", 
            fun () -> 
                self.Navigation.PopAsync()
                |> Async.AwaitTask
                |> Async.Ignore
                |> Async.StartImmediate)

    let layout =
        let layout = new StackLayout(Padding = new Thickness(10.))
        layout.Children.Add(date)
        layout.Children.Add(time)
        layout

    do
        // Bindings
        date.SetBinding(DatePicker.DateProperty, "Date")            
        time.SetBinding(TimePicker.TimeProperty, "Time")
        self.SetBinding(ContentPage.TitleProperty, "Title") 
        save.SetBinding(ToolbarItem.CommandProperty, "UpdateCommand")

        // Toolbar items
        base.ToolbarItems.Add(save)

        base.BindingContext <- vm
        base.Content <- layout
