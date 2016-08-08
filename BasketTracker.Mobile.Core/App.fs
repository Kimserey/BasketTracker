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
        { Navigation = nav.Navigation
          Store =
            { NavigateToAdd = 
                fun nav (Context ctx) ->
                    let vm = new AddStoreViewModel(ctx :?> StoreListViewModel, Stores.api, "Add new store")
                    let page = new AddStorePage(vm)
                    nav.Navigation.PushAsync(page)
                    |> Async.AwaitTask 
                    |> Async.StartImmediate  
              
              NavigateToUpdate =
                fun nav (Context ctx) ->
                    let parent = ctx :?> StoreCellViewModel
                    let vm = new UpdateStoreViewModel(parent, Stores.api, "Update an existing store", parent.Store)
                    let page = new UpdateStorePage(vm)
                    nav.Navigation.PushAsync(page)
                    |> Async.AwaitTask 
                    |> Async.StartImmediate }

          Basket = 
            { NavigateToBasketList =
                fun nav (Context ctx) ->
                    let parent = ctx :?> StoreCellViewModel
                    let vm = new BasketListViewModel()
                    let page = new BasketListPage(vm, nav)
                    nav.Navigation.PushAsync(page)
                    |> Async.AwaitTask
                    |> Async.StartImmediate
              
              NavigateToAdd = fun nav (Context ctx) -> ()
              NavigateToUpdate = fun nav (Context ctx) -> () } }

    let vm =
        new StoreListViewModel(
            title = "Stores", 
            api = Stores.api)

    do 
        nav.PushAsync(new StoreListPage(vm, navigator))
        |> Async.AwaitTask
        |> Async.StartImmediate

        base.MainPage <- nav
            