## Bank

This mod adds common banking options for items, currency, and luminance.



The verbs available are currently:

* `list` - print current status
* `give`
* `take`



The available commands are:

* `/bank <verb> [name|id [amount=1|*]]`
  * Bankable `Items` can be defined in the `Settings.json` with a name, WCID, and Id to use to store them as a `PropertyInt64` 
  * `/bank list` prints bankable items, with the amount stored and held
  * `/bank store [name|id [amount=1|*]]` stores a specified amount or all available of an `Item` matching part of the name or all of the ID
  * `/bank take [name|id [amount=1|*]]` does the opposite of store
* `/cash <verb> [name]`
  * `Currencies` items can be defined in `Settings.json` with a name, WCID, and cost.  Deposited items do not use these values.
  * `/cash list` prints balance along with items available for stored currency
  * `/cash give` deposits all pyreals and Trade Notes
  * `/cash take [name] [amount=1]` withdraws an amount of the named item
* `/lum <verb>`
  * `/lum give` deposits all
  * `/lum take` withdraws enough to hit max if available







### Settings

* `DirectDeposit` will cause coin sales to be added to the bank instead of using pyreal stacks
  * `/ddt` will toggle a players use of this feature in case the server wants it enabled but a player does not

* `VendorsUseBank` will use banked items to complete the transaction first
* `ExcessSetToMax` will reduce a quantity exceeding the max to the max amount






### Demos

https://github.com/aquafir/ACE.BaseMod/assets/83029060/57d40ac2-2998-4fac-8bd5-af64ba281192



https://github.com/aquafir/ACE.BaseMod/assets/83029060/09161459-0ed1-4a43-9c4c-c41b81f6142c



https://github.com/aquafir/ACE.BaseMod/assets/83029060/fbee854d-5859-4b1b-9594-073891ed3e7d



https://github.com/aquafir/ACE.BaseMod/assets/83029060/fa47f292-5065-4d75-ae87-4d9cfe811697



https://github.com/aquafir/ACE.BaseMod/assets/83029060/f7932ee2-1dd6-440a-b5d7-057b9b9f2c9b



https://github.com/aquafir/ACE.BaseMod/assets/83029060/692a2488-0a27-4f04-9a11-566495c17a77

