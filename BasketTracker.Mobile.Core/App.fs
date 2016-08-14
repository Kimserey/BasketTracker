﻿namespace BasketTracker.Mobile.Core

open System
open System.Collections
open System.Collections.Generic
open System.ComponentModel
open Xamarin.Forms
open SQLite.Net
open BasketTracker.Mobile.Core.Models
open BasketTracker.Mobile.Core.Storage
open BasketTracker.Mobile.Core.Stores
open BasketTracker.Mobile.Core.Baskets
open BasketTracker.Mobile.Core.Items

type AboutPage() =
    inherit ContentPage(Title = "About")


    do
        base.Content <- new ScrollView()

type App() = 
    inherit Application()
    
    let nav =
        new NavigationPage()

    let navigator = 
        { Navigation = nav.Navigation
          Store =
            { NavigateToAdd = 
                fun nav (Context ctx) ->
                    let vm = new AddStoreViewModel(ctx :?> StoreListViewModel, Stores.Storage.api, "Add new store")
                    let page = new AddStorePage(vm)
                    nav.PushModal page
              
              NavigateToUpdate =
                fun nav (Context ctx) ->
                    let vm = new UpdateStoreViewModel(ctx :?> StoreCellViewModel, Stores.Storage.api, "Update an existing store")
                    let page = new UpdateStorePage(vm)
                    nav.PushModal page }

          Basket = 
            { NavigateToBasketList =
                fun nav (Context ctx) ->
                    let parent = ctx :?> StoreCellViewModel
                    let vm = new BasketListViewModel(parent.Id, parent.Name, Baskets.Storage.api)
                    let page = new BasketListPage(vm, nav)
                    nav.Navigate(page)
              
              NavigateToAdd = 
                fun nav (Context ctx) -> 
                    let parent = ctx  :?> BasketListViewModel
                    let vm = new AddBasketViewModel(parent, Baskets.Storage.api)
                    let page = new AddBasketPage(vm)
                    nav.Navigate(page)

              NavigateToUpdate = 
                fun nav (Context ctx) -> 
                    let parent = ctx :?> BasketCellViewModel
                    let vm = new UpdateBasketViewModel(parent, Baskets.Storage.api)
                    let page = new UpdateBasketPage(vm)
                    nav.Navigate(page) }
         
          Item =
            { NavigateToItemList =
                fun nav (Context ctx) -> 
                    let parent = ctx :?> BasketCellViewModel
                    let vm = new ItemListViewModel(parent.Id, parent.Date, Items.Storage.api)
                    let page = new ItemListPage(vm, nav)
                    nav.Navigate(page)

              NavigateToAdd =
                fun nav (Context ctx) -> 
                    let parent = ctx  :?> ItemListViewModel
                    let vm = new AddItemViewModel(parent, Items.Storage.api)
                    let page = new AddItemPage(vm)
                    nav.Navigate(page)

              NavigateToUpdate = 
                fun nav (Context ctx) -> 
                    let parent = ctx :?> ItemCellViewModel
                    let vm = new UpdateItemViewModel(parent, Items.Storage.api)
                    let page = new UpdateItemPage(vm)
                    nav.Navigate(page) } }

    let vm =
        new StoreListViewModel(Stores.Storage.api)

    do 
        nav.PushAsync(new StoreListPage(vm, navigator))
        |> Async.AwaitTask
        |> Async.StartImmediate
        
        base.MainPage <- nav
            