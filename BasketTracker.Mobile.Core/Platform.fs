namespace BasketTracker.Mobile.Core

open System
open Xamarin.Forms

type INavigator =
    abstract Navigation: INavigation
    abstract Store:      IStoreNavigator
    abstract Basket:     IBasketNavigator 
and IStoreNavigator =
    abstract NavigateToStoreList:  nav:INavigation -> context:obj -> unit
    abstract NavigateToStore:      nav:INavigation -> context:obj -> unit
    abstract NavigateToCreate:     nav:INavigation -> context:obj -> unit
    abstract NavigateToEdit:       nav:INavigation -> context:obj -> unit
and IBasketNavigator =
    abstract NavigateToBasketList: nav:INavigation -> context:obj -> unit
    abstract NavigateToBasket:     nav:INavigation -> context:obj -> unit
    abstract NavigateToCreate:     nav:INavigation -> context:obj -> unit
    abstract NavigateToEdit:       nav:INavigation -> context:obj -> unit

type IPathsProvider =
    abstract member ApplicationData : string