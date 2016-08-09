namespace BasketTracker.Mobile.Core

open Xamarin.Forms
open Models

type Context = Context of obj

type Navigator = {
    Navigation: INavigation
    Store: StoreNavigator
    Basket: BasketNavigator 
    Item: ItemNavigator
}

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
}
