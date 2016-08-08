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
                    member store.NavigateToAdd nav ctx = 
                        let vm = new AddStoreViewModel("Add new store", Stores.add)
                        let page = new AddStorePage(vm)
                        nav.Navigation.PushAsync(page)
                        |> Async.AwaitTask 
                        |> Async.StartImmediate  

                    member store.NavigateToUpdate nav ctx = 
                        let ctx = ctx :?> StoreCellViewModel
                        let vm = new UpdateStoreViewModel("Update an existing store", ctx.Name, Stores.update ctx.Id)
                        let page = new UpdateStorePage(vm)
                        nav.Navigation.PushAsync(page)
                        |> Async.AwaitTask 
                        |> Async.StartImmediate   }

            member self.Basket = 
                { new IBasketNavigator with
                    member basket.NavigateToBasketList nav ctx = ()
                    member basket.NavigateToAdd     nav ctx = ()
                    member basket.NavigateToUpdate       nav ctx = () } }

    do 
        nav.PushAsync(new StoreListPage(new StoreListViewModel(title = "Stores", listStores = Stores.list, archiveStore = Stores.archive), navigator))
        |> Async.AwaitTask
        |> Async.StartImmediate

        base.MainPage <- nav
            