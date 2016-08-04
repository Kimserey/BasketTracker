namespace BasketTracker.Mobile.Core

open System
open Storage

module Model =

    type StoreList(connectionFactory: SQLiteConnectionFactory) =
        
        member self.List() =
            use conn = connectionFactory()
            SQLStore.List conn
            |> List.map (fun s -> new Store(s, connectionFactory))

        member self.Add name =
            use conn = connectionFactory()
            SQLStore.Add name conn

    and Store(store, connectionFactory: SQLiteConnectionFactory) =
        
        let mutable name = store.Name
        let mutable archived = store.Archived
        
        member self.Name
            with get() = name
            and private set value =
                name <- value

        member self.Archived
            with get() = archived
            and private set value =
                archived <- value

        member self.Archive() =
            use conn = connectionFactory()
            SQLStore.Archive store.Id conn
            self.Archived <- true

        member self.UpdateName name =
            use conn = connectionFactory()
            SQLStore.Update store.Id name conn
            self.Name <- name

        member self.BasketList() =
            use conn = connectionFactory()
            SQLBasketQueryResult.List store.Id conn
            |> List.map (fun b -> new BasketSummary(b, connectionFactory))

    and BasketSummary(basket, connectionFactory: SQLiteConnectionFactory) =
        
        let mutable date = basket.Date
        let mutable total = basket.Total

        member self.Date
            with get() = date

        member self.Total
            with get() = total