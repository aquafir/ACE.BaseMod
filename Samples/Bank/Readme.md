## Bank

This mod adds common banking options for items, currency, and luminance.



The verbs available are currently:

* `list` - print current status
* `give`
* `take`



The available commands are:

* `/lum <verb>`
  * `/lum give` deposits all
  * `/lum take` withdraws enough to hit max if available
* `/cash <verb> [name]`
  * `Currencies` items can be defined in `Settings.json` with a name, WCID, and cost.  Deposited items do not use these values.
  * `/cash list` prints balance along with items available for stored currency
  * `/cash give` deposits all pyreals and Trade Notes
  * `/cash take [name] [amount=1]` withdraws an amount of the named item
* `/bank <verb> [name|id [amount=1|*]]`
  * Bankable `Items` can be defined in the `Settings.json` with a name, WCID, and Id to use to store them as a `PropertyInt64` 
  * `/bank list` prints bankable items, with the amount stored and held
  * `/bank store [name|id [amount=1|*]]` stores a specified amount or all available of an `Item` matching part of the name or all of the ID
  * `/bank take [name|id [amount=1|*]]` does the opposite of store





### Demos

