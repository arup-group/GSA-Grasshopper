# Unit number

| Icon                                  |
| ------------------------------------- |
| ![UnitNumber](./images/UnitParam.png) |

## Description

GSA-Grasshopper includes a new way to work with units inside Grasshopper; we call this parameter a _UnitNumber_. For components that are using a _UnitNumber_ as input parameter you can simply input a number as usual in GSA and the component will automatically convert it into the selected unit on the component's dropdown.

:::info Definition

A _UnitNumber_ is technically a Quantity defined as:

$Quantity = Value \times Measure.$

:::

We are using the opensource library [UnitsNet](https://github.com/angularsen/UnitsNet) and have extended it with engineering units. The project is opensource, and should you require a unit that is not supported you can add it to either UnitsNet or [OasysUnits](https://github.com/arup-group/oasysunits/).

## Create unit number

The following units have been exposed (many more are available in UnitsNet, let us know if you need them exposed...):

- `Angle`
- `Length`
- `Area`
- `Volume`
- `AreaMomentOfInertia`
- `Force`
- `ForcePerLength`
- `ForcePerArea`
- `Moment`
- `Stress`
- `Strain`
- `AxialStiffness`
- `BendingStiffness`
- `Curvature`
- `Mass`
- `Density`
- `Temperature`
- `Velocity`
- `Acceleration`
- `Energy`
- `Ratio`
- `Time`
- `LinearDensity`
- `VolumePerLength`
- `SectionModulus`

As mentioned, most components having _UnitNumber_ inputs will also have a dropdown for you to select the unit . You can input a normal slider component to create a _UnitNumber_ input of the selected unit, or you can use the _Create UnitsNumber_ component to create (and mix) your own:
![CreateUnitNumber](./images/gsagh-create-unitnumber.gif)

## Convert unit number

Aside from creating a Unit Number from scratch, you can also convert a `UnitNumber` between units of the same type. This is handy for results that you may want to convert into a more familiar unit.

![ConvertUnitNumber](./images/gsagh-convert-unitnumber.gif)
