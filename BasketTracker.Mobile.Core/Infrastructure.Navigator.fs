namespace BasketTracker.Mobile.Core

open Models
open Xamarin.Forms

type Context = Context of obj

type Navigator = {
    Navigation: INavigation
    Store: StoreNavigator
    Basket: BasketNavigator 
    Item: ItemNavigator
} with
     member self.Navigate(page) =
        self.Navigation.PushAsync(page)
        |> Async.AwaitTask 
        |> Async.StartImmediate  

     member self.PushModal(page) =
        self.Navigation.PushModalAsync(page)
        |> Async.AwaitTask
        |> Async.StartImmediate

and StoreNavigator = {
    NavigateToAdd: Navigator -> Context -> unit
    NavigateToUpdate: Navigator -> Context -> unit
}

and BasketNavigator = {
    NavigateToBasketList: Navigator -> Context -> unit
    NavigateToAdd: Navigator -> Context -> unit
    NavigateToUpdate: Navigator -> Context-> unit
}

and ItemNavigator = {
    NavigateToItemList: Navigator -> Context -> unit
    NavigateToAdd: Navigator -> Context -> unit
    NavigateToUpdate: Navigator -> Context -> unit
}
