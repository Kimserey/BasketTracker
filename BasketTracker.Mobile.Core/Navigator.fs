namespace BasketTracker.Mobile.Core

open Xamarin.Forms

type INavigator =
    abstract Navigation: INavigation
    abstract Store:      IStoreNavigator
    abstract Basket:     IBasketNavigator 
and IStoreNavigator =
    abstract NavigateToStoreList:  nav:INavigator -> context:obj -> unit
    abstract NavigateToStore:      nav:INavigator -> context:obj -> unit
    abstract NavigateToCreate:     nav:INavigator -> context:obj -> unit
    abstract NavigateToEdit:       nav:INavigator -> context:obj -> unit
and IBasketNavigator =
    abstract NavigateToBasketList: nav:INavigator -> context:obj -> unit
    abstract NavigateToBasket:     nav:INavigator -> context:obj -> unit
    abstract NavigateToCreate:     nav:INavigator -> context:obj -> unit
    abstract NavigateToEdit:       nav:INavigator -> context:obj -> unit