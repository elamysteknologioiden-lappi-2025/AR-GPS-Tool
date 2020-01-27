# AR-GPS-Tool
Tool for Unity to place 3D-objects into a AR-world based on GPS-coordinates.

# Requirements
- Requires Unity's AR Foundation. Project uses AR Foundation version 3 but it should also work with version 2 with minor modifications.
- AR Foundation/ARCore requires Android API level 24 (Android 7.0 'Nougat').
- Android smart phone must be in the [ARCore supported devices list](https://developers.google.com/ar/discover/supported-devices)

# Version
1.0.0

# Guide
"Demo Scene" is an example scene on how to setup the tool.
"AR GPS Tool" prefab contains the main components needed.

Before use, see guides how to setup and use AR Foundation.

## Components/Scripts

### AR Point of Interest Manager
Handles the calculations and positioning of point of interests. [Point of Interest Set](#Point-of-Interests) must be assigned containg all of the point of interests which to track.

### Location Provider
Provides updates about the device's GPS-location. Emits events containing the location and the accuracy.

### Heading Provider
Provides updates about the device's compass readings. Emits events containing the raw compass heading and filtered/smoothed compass heading (eliminate jitter).

### AR True North Finder
Uses the [Location Provider](#Location-Provider) and [Heading Provider](#Heading-Provider) to calculate north to align the real world with the Unity AR-world. It can be calculated either manually, using the compass, using the GPS differences and AR-movement differences, or using both the compass and GPS/AR-differences.

### AR Device Elevation Estimater
Uses the instantiated AR planes to get an estimate about the elevation of the device (relative to ground) and where the ground level is. AR Point of Interest Manager uses this data to align objects on on the ground level, or relative to it.

### AR Disable Planes On Tracking Lost
Disables all AR-planes if the tracking is lost.

## Point of Interests
### Point of Interest Set
ScriptableObject that holds a list of Point of Interests.

### Point of Interest
ScriptableObject that holds details of one Point of Interest.
 - Name
 - Description
 - GPS-coordinates
 - Tracking distances
 - Positioning (vertical)
 - Rotation
 - Object Prefab: Parent object that has a component that is inherited from PointOfInterestObjectBase. The component holds the logic for how to position the 3D-object.
 - Model Prefab: The actual 3D-model of the point of interest which will be instantiated.
 - Canvas Prefab: Canvas object that has a component that is inherited from PointOfInterestCanvasBase. The component holds the logic for how to position and display the canvas marker.

# Elämysteknologioiden Lappi 2025

Uudet digitaaliset teknologiat tarjoavat lappilaisille, etenkin matkailualan yrityksille mahdollisuuksia, joita ei tällä hetkellä hyödynnetä täysipainoisesti. Digitalisaatio on tuonut matkapuhelimet, sovellukset ja internetsivut osaksi normaalia arkea ja näitä toteutuksia matkailuyritykset osaavat jo toteuttaa. Elämysteknologioiden Lappi 2025 hankkeessa katsotaan näiden toteutuksien yläpuolelle ja tutkitaan uudenlaisia elämyksiä sekä niihin pohjautuvia tuotteita ja palveluita mahdollistavaa teknologiaa, joka ei ole vielä yrityksille ja toimijoille tuttua.

Hankkeen kokonaiskustannukset ovat 292 551 euroa, josta Lapin liitto on myöntänyt Euroopan aluekehitysrahaston (EAKR) ja valtion rahoitusta 219 414 euroa

# Lapland Experience Technologies 2025
The long-term objective in this project is to make the potential of new experience technologies more noticeable andmore reachable to businesses in Lapland. The project’s primary objective for achieving this is to develop a digitaltoolbox for  VR/AR/MR ( VirtualReality, AugmentedReality/ MixedReality ), that is designed based on the needs of thecompanies in the area, and that will enable faster and easier usage of the new experience technologies in their trade.The “experience technology toolbox” is distributed online for the companies to freely use. The tools that it contains areused to create two proof-of-concept demos. These demos are then introduced to companies for demonstrating their potential in business, and workshops are run with the companies to produce ideas and concepts of new products andservices that are based on the used experience technologies.The project enriches the cooperation between the companies, the public and the educational organisations in the area.The project also develops the cooperation between the ICT department of Lapland University of Applied Sciences, theIndustrial design department of Lapland University and the Multidimensional Tourism Institute, and makes thepossibilities of the cooperation more visible. The educational organisations also aim to improve the cooperation withthe local companies by creating new relationships with them and by making the cooperation more multi-disciplined.

![Screenshot](LapanenLogo.jpg)
