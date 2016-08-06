namespace BasketTracker.Mobile.Core

open System

module Models =

    type Store = {
        Id: int
        Name: string
    }

    type Basket = {
        Id: int
        StoreId: int
        Date: DateTime
        Total: decimal
    }