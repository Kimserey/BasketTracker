namespace BasketTracker.Mobile.Core

open System

module Domain =

    type Stores = Stores of Store list
        with
            static member Sample = 
                Stores [
                    { Name = "Asda"
                      Baskets = 
                        [
                            { Date = DateTime.Now
                              Items = 
                                [
                                    { Name = "Apple"
                                      Amount = 2.05m}
                                    { Name = "Bread"
                                      Amount = 3.5m}
                                ] }
                             
                            { Date = DateTime.Now.AddDays(-1.)
                              Items = 
                                [
                                  { Name = "Apple"
                                    Amount = 2.05m}
                                  { Name = "Bread"
                                    Amount = 3.5m}
                                ] }
                        ]}

                    { Name = "Tesco"
                      Baskets =
                        [
                            { Date = DateTime.Now
                              Items =
                                [
                                  { Name = "Apple"
                                    Amount = 2.05m}
                                  { Name = "Bread"
                                    Amount = 3.5m}
                                ] }
                             
                            { Date = DateTime.Now.AddDays(-1.)
                              Items = 
                                [
                                  { Name = "Apple"
                                    Amount = 2.05m}
                                  { Name = "Bread"
                                    Amount = 3.5m}
                                ] }
                        ]}

                    { Name = "Waitrose"
                      Baskets =
                        [
                            { Date = DateTime.Now
                              Items = 
                                [
                                  { Name = "Apple"
                                    Amount = 2.05m}
                                  { Name = "Bread"
                                    Amount = 3.5m}
                                ] }
                             
                            { Date = DateTime.Now.AddDays(-1.)
                              Items = 
                                [
                                  { Name = "Apple"
                                    Amount = 2.05m}
                                  { Name = "Bread"
                                    Amount = 3.5m}
                                ] }
                        ]}
                ]
         
    and Store = {
        Name: string
        Baskets: Basket list
    } 

    and Basket = {
        Date: DateTime
        Items: BasketItem list
    }

    and BasketItem = {
        Name: string
        Amount: decimal
    }