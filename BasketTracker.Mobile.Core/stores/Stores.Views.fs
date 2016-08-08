namespace BasketTracker.Mobile.Core.Stores

open BasketTracker.Mobile.Core
open Xamarin.Forms
open System

module Views =

    type StoreListPage(vm, navigator: Navigator) as self =
        inherit ContentPage()

        let listView = 
            new ListView(ItemTemplate = new DataTemplate(fun () -> box (new StoreViewCell(navigator))))
            
        let add = 
            new ToolbarItem(
                "Add new store", 
                "shop_add", 
                fun () -> navigator.Store.NavigateToAdd navigator <| Context self.BindingContext)
    
        do
            // Bindings            
            self.SetBinding(ContentPage.TitleProperty, "Title")
            listView.SetBinding(ListView.ItemsSourceProperty, "List")

            // Toolbar items
            self.ToolbarItems.Add(add)

            base.BindingContext <- vm
            self.Content <- listView
            
    and StoreViewCell(navigator: Navigator) as self =
        inherit ViewCell()
    
        let name    = new Label(YAlign = TextAlignment.Center)
        let shop    = new Image(Source = FileImageSource.op_Implicit "shop")
        let update    = new MenuItem(Text = "Edit", Icon = FileImageSource.op_Implicit "pencil")
        let remove  = new MenuItem(Text = "Remove", Icon = FileImageSource.op_Implicit "bin")

        let layout = 
            let layout = new Grid()
            layout.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(1., GridUnitType.Star)))
            layout.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(3., GridUnitType.Star)))
            layout.Children.Add(shop, 0, 0)
            layout.Children.Add(name, 1, 0)
            layout

        do
            // Navigation events
            self.Tapped.Add(fun _ -> navigator.Basket.NavigateToBasketList navigator <| Context self.BindingContext)
            update.Clicked.Add(fun _ -> navigator.Store.NavigateToUpdate navigator<| Context  self.BindingContext)
            
            // Bindings
            name.SetBinding(Label.TextProperty, ".Name")
            remove.SetBinding(MenuItem.CommandProperty, "RemoveCommand")

            // Context actions
            self.ContextActions.Add(update)
            self.ContextActions.Add(remove)

            self.View <- layout


    type AddStorePage(vm: obj) as self =
        inherit ContentPage()

        let entry = new Entry(Placeholder = "Enter a store name here")
        let save =
            new ToolbarItem(
                "Save this store", 
                "save", 
                fun () -> 
                    self.Navigation.PopAsync()
                    |> Async.AwaitTask
                    |> Async.Ignore
                    |> Async.StartImmediate)

        let layout = 
            let layout = new StackLayout()
            layout.Children.Add(entry)
            layout

        do
            // Bindings
            self.SetBinding(ContentPage.TitleProperty, "Title")
            entry.SetBinding(Entry.TextProperty, "Name")
            save.SetBinding(ToolbarItem.CommandProperty, "AddCommand")
            save.SetBinding(ToolbarItem.CommandParameterProperty, "Name")

            // Toolbar items
            base.ToolbarItems.Add(save)

            base.BindingContext <- vm
            base.Content <- layout


    type UpdateStorePage(vm: obj) as self =
        inherit ContentPage()

        let entry =
            new Entry(Placeholder = "Enter a store name here")

        let save =
            new ToolbarItem(
                "Save your changes", 
                "save", 
                fun () -> 
                    self.Navigation.PopAsync()
                    |> Async.AwaitTask
                    |> Async.Ignore
                    |> Async.StartImmediate)

        let layout =
            let layout = new StackLayout()
            layout.Children.Add(entry)
            layout

        do
            // Bindings
            self.SetBinding(ContentPage.TitleProperty, "Title")
            entry.SetBinding(Entry.TextProperty, "Name")
            save.SetBinding(ToolbarItem.CommandProperty, "UpdateCommand")
            save.SetBinding(ToolbarItem.CommandParameterProperty, "Name")

            // Toolbar items
            self.ToolbarItems.Add(save)

            base.BindingContext <- vm
            self.Content <- layout
