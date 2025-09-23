# Spring Property
<!--- This file has been auto-generated, do not change it manually! Edit the generator here: https://github.com/arup-group/GSA-Grasshopper/tree/main/DocsGeneration --->

|<img width="150"/> Icon |
| ----------- |
|![SpringPropertyParam](./images/SpringPropertyParam.png) |

## Description

A spring is a general type of element which can be used to model both simple springs and more sophisticated types of behaviour. Spring properties describe those behaviours. Refer to [Spring Properties](/references/hidr-data-pr-spring/) to read more.

## Properties

|<img width="20"/> Icon |<img width="200"/> Type |<img width="200"/> Name |<img width="1000"/> Description |
| ----------- | ----------- | ----------- | ----------- |
|![TextParam](./images/TextParam.png) |`Text` |**Name** |Spring Property Name |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Spring Curve x** |Spring Curve in x direction |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force Per Length ` |**Stiffness x** |Stiffness in x direction |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Spring Curve y** |Spring Curve y |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force Per Length ` |**Stiffness y** |Stiffness in y direction |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Spring Curve z** |Spring Curve z |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force Per Length ` |**Stiffness z** |Stiffness in z direction |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Spring Curve xx** |Spring Curve xx |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Rotational Stiffness ` |**Stiffness xx** |Stiffness in xx direction |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Spring Curve yy** |Spring Curve yy |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Rotational Stiffness ` |**Stiffness yy** |Stiffness in yy direction |
|![IntegerParam](./images/IntegerParam.png) |`Integer` |**Spring Curve zz** |Spring Curve zz |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Rotational Stiffness ` |**Stiffness zz** |Stiffness in zz direction |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Spring Matrix** |Spring Matrix |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Lockup -ve** |Lockup -ve |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Length ` |**Lockup +ve** |Lockup +ve |
|![UnitNumber](./images/UnitParam.png) |[Unit Number](gsagh-unitnumber-parameter.md)  ` Force Per Length ` |**Coeff. of Friction** |Coefficient of Friction |
|![GenericParam](./images/GenericParam.png) |`Generic` |**Damping Ratio** |[Optional] Damping Ratio (Default = 0.0 -> 0%) |

_Note: the above properties can be retrieved using the [Get Spring Property](gsagh-get-spring-property-component.md) component_
