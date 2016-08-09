namespace BasketTracker.Mobile.Core

open System

module Models =
    
    type Store = {
        Id: StoreId
        Name: string
    }
    and StoreId = StoreId of int

    type Basket = {
        Id: BasketId
        StoreId: StoreId
        Date: DateTime
        Total: decimal
    }
    and BasketId = BasketId of int

    type Item = {
        Id: ItemId
        BasketId: BasketId
        Name: string
        Amount: decimal
    }
    and ItemId = ItemId of int