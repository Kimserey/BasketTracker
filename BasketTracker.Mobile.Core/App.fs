namespace BasketTracker.Mobile.Core

open System
open System.Collections
open System.Collections.Generic
open System.ComponentModel
open Xamarin.Forms
open Storage
open SQLite.Net
open Models
open BasketTracker.Mobile.Core.Stores.ViewModels
open BasketTracker.Mobile.Core.Stores.Views
open BasketTracker.Mobile.Core.Baskets.ViewModels
open BasketTracker.Mobile.Core.Baskets.Views

type App() = 
    inherit Application()
    
    let nav =
        new NavigationPage()

    let navigator = 
        { new INavigator with
            member self.Navigation =
                nav.Navigation

            member self.Store = 
                { new IStoreNavigator with
                    member store.NavigateToStoreList nav ctx = ()
                    member store.NavigateToStore     nav ctx = ()
                    member store.NavigateToCreate    nav ctx = ()
                    member store.NavigateToEdit      nav ctx = () }

            member self.Basket = 
                { new IBasketNavigator with
                    member basket.NavigateToBasketList nav ctx = ()
                    member basket.NavigateToBasket     nav ctx = ()
                    member basket.NavigateToCreate     nav ctx = ()
                    member basket.NavigateToEdit       nav ctx = () } }

    do 
        nav.PushAsync(new StoreListPage(new StoreListViewModel(title = "Stores", listStores = Stores.list, archiveStore = Stores.archive), navigator))
        |> Async.AwaitTask
        |> Async.StartImmediate

        base.MainPage <- nav
            