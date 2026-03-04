# List
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

| <img width="150"/> Icon               |
| ------------------------------------  |
| ![ListParam](./images/ListParam.png)  |

## Description

An Entity List is expressed as a string of text in a specific syntax along with the List Type. In Grasshopper, a Entity List can also contain a copy of all the items in the list. 

Lists (of nodes, elements, members or cases) are used, for example, when a particular load is to be applied to one or several elements. To define a series of items the list can either specify each individually or, if applicable, use a more concise [syntax](/references/listsandembeddedlists.md).



## Properties

| <img width="20"/> Icon                     | <img width="200"/> Type        | <img width="200"/> Name        | <img width="1000"/> Description                       |
| ------------------------------------------ | ------------------------------ | ------------------------------ | ----------------------------------------------------  |
| ![IntegerParam](./images/IntegerParam.png) | `Integer`                      | **Index**                      | List Number if the list ever belonged to a GSA Model  |
| ![TextParam](./images/TextParam.png)       | `Text`                         | **Name**                       | List Name                                             |
| ![TextParam](./images/TextParam.png)       | `Text`                         | **Type**                       | Entity Type                                           |
| ![TextParam](./images/TextParam.png)       | `Text`                         | **Definition**                 | List Definition                                       |
| ![GenericParam](./images/GenericParam.png) | `Generic` _List_               | **List Objects**               | Expanded objects contained in the input list          |
| ![IntegerParam](./images/IntegerParam.png) | `Integer` _List_               | **Expand List**                | Expanded list IDs                                     |

_Note: the above properties can be retrieved using the [List Info](gsagh-list-info-component.md) component_
