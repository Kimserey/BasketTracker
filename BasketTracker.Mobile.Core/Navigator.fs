namespace BasketTracker.Mobile.Core

open Xamarin.Forms

type INavigator =
    abstract Navigation: INavigation
    abstract Store:      IStoreNavigator
    abstract Basket:     IBasketNavigator 
and IStoreNavigator =
    abstract NavigateToAdd:        nav:INavigator -> context:obj -> unit
    abstract NavigateToUpdate:     nav:INavigator -> context:obj -> unit
and IBasketNavigator =
    abstract NavigateToBasketList: nav:INavigator -> context:obj -> unit
    abstract NavigateToAdd:        nav:INavigator -> context:obj -> unit
    abstract NavigateToUpdate:     nav:INavigator -> context:obj -> unit