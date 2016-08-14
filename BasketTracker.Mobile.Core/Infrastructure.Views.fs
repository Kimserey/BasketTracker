namespace BasketTracker.Mobile.Core

open Xamarin.Forms

type ModalPage() as self =
    inherit ContentPage()

    let positive = new Button(Text = "Save")
    let negative = new Button(Text = "Cancel")

    let popModal() =
        self.Navigation.PopModalAsync()
        |> Async.AwaitTask
        |> Async.Ignore
        |> Async.StartImmediate

    do
        negative.Clicked.Add(ignore >> popModal)
        positive.Clicked.Add(ignore >> popModal)
        
    member self.Positive
        with get() = positive
        
    member self.Negative
        with get() = negative

    member self.MakeLayout view = 
        let layout = new StackLayout(Padding = new Thickness(10.))
        layout.Children.Add(view)

        let buttonLayout = new Grid()
        buttonLayout.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(1., GridUnitType.Star)))
        buttonLayout.ColumnDefinitions.Add(new ColumnDefinition(Width = new GridLength(1., GridUnitType.Star)))
        buttonLayout.Children.Add(negative, 0, 0)
        buttonLayout.Children.Add(positive, 1, 0)
        layout.Children.Add(buttonLayout)
        
        layout
