namespace BasketTracker.Mobile.Core

open System
open System.Collections
open System.Collections.Generic
open System.ComponentModel
open Xamarin.Forms
open Storage
open SQLite.Net
open Models
open BasketTracker.Mobile.Core.Stores.Views

type App() = 
    inherit Application()
    
    let addPage = 
        new AddStorePage()
    
    let updateStorePage = 
        new UpdateStorePage()

    let basketListPage = 
        new BasketListPage(
            goToAdd =
                (fun nav -> ()),
            goToBasket =
                (fun nav basket -> ()))

    let page = 
        new StoreListPage<StoreCellViewModel>(
            getStoreList = 
                Stores.list,
            goToAdd = 
                (fun nav ->
                    addPage.BindingContext <- new AddStoreViewModel("Add a new store", Stores.add)
                    nav.PushAsync(addPage)
                    |> Async.AwaitTask
                    |> Async.StartImmediate),
            goToEdit =
                (fun nav ctx ->
                    updateStorePage.BindingContext <- new UpdateStoreViewModel("Update the store name", ctx.Name, Stores.update ctx.Id)
                    nav.PushAsync(updateStorePage)
                    |> Async.AwaitTask
                    |> Async.StartImmediate),
            goToBaskets = 
                (fun nav ctx ->
                    basketListPage.BindingContext <- new BasketListViewModel(ctx.Name, (fun () -> Baskets.list ctx.Id))
                    nav.PushAsync(basketListPage)
                    |> Async.AwaitTask
                    |> Async.StartImmediate)
        )

    do 
        page.BindingContext <- 
            new StoreListViewModel(
                title = "Stores", 
                getList = Stores.list, 
                archiveStore = Stores.archive
            )

        base.MainPage <- new NavigationPage(page)