using UnityEngine;
using System.Collections;
using System.IO;

public class CityGenerator : MonoBehaviour {

    /*
     * TODO
     *  - Setup a means of defining how the city is divided beyond just a plain grid
     *      - what constitutes a subsection (see _buildCity)
     */

    /*
     * Control variables
     */

    // width and height of city
    private int _cityWidth = 64, _cityHeight = 64;
    // width and height (2d) of park
    private int _parkFootprintWidth = 2, _parkFootprintHeight = 4;  // ** width should probably be 2 as that is the width of buildings in a section
    // number of parks in city
    private int _numberOfParks = 2;

    /*\
     * Enum for identifying areas
     */

    public enum FILLTYPE {
        BULIDING = 0,
        ROAD,
        PARK
    };
 
    /*
     * Starter functions
     */

    public FILLTYPE[,] buildCity() {
        return _buildCity();
    }

    public FILLTYPE[,] buildCity(int cityWidth, int cityHeight) {
        _cityWidth = cityWidth;
        _cityHeight = cityHeight;
        return _buildCity();
    }

    public FILLTYPE[,] buildCity(int cityWidth, int cityHeight,int parkWidth, int parkHeight, int parkCount) {
        _cityWidth = cityWidth;
        _cityHeight = cityHeight;
        _parkFootprintWidth = parkWidth;
        _parkFootprintHeight = parkHeight;
        _numberOfParks = parkCount;
        return _buildCity();
    }

    /*
     * Getters and setters for control variables
     */
    
    public void setCitySize(int width, int height) {
        _cityWidth = width;
        _cityHeight = height;
    }

    public int getCityWidth() {
        return _cityWidth;
    }

    public int getCityHeight() {
        return _cityHeight;
    }

    public void setParkFootprint(int width, int height) {
        _parkFootprintWidth = width;
        _parkFootprintHeight = height;
    }

    public int getParkFootprintWidth() {
        return _parkFootprintWidth;
    }

    public int getParkFootprintHeight() {
        return _parkFootprintHeight;
    }

    public void setParkNumber(int parks) {
        _numberOfParks = parks;
    }

    public int getNumberOfParks() {
        return _numberOfParks;
    }

    /*
     * Functions for actually generating the city
     * ***CHANGED TO PUBLIC AND HAS RETURN TYPE ***
     * 
     * **** There are public functions above which should be used to kick off
     *       the process. ****
     */

    private FILLTYPE[,] _buildCity() {

        // Enums are initialized to 0 value (which is BUILDING) by default
        FILLTYPE[,] cityGrid = new FILLTYPE[_cityHeight, _cityWidth];  // note height by width

        int width, height;

        // one subsection is a building then road then building
        int subsectionWidth = 3;
        int numberOfSubsectionsAcross = _cityWidth / subsectionWidth;

        // for now, lets say a subsection of height is 6 buildings then a road
        int subsectionHeight = 7;
        int numberOfSubsectionsUp = _cityHeight / subsectionHeight;

        // Cut vertical roads
        width = 1;
        for (int i = 0; i < numberOfSubsectionsAcross; ++i) {
            for (height = 1; height < _cityHeight - 1; ++height) {
                cityGrid[height, width] = FILLTYPE.ROAD;
            }

            width += subsectionWidth;
        }

        // Cut horizontal roads
        height = 1;
        for (int i = 0; i < numberOfSubsectionsUp; ++i) {
            for (width = 1; width < _cityWidth - 1; ++width) {
                cityGrid[height, width] = FILLTYPE.ROAD;
            }

            height += subsectionHeight;
        }

        int newEdgeVert, newEdgeHoriz;
        // Need the new edges of the city so we don't place things outside
        // The number will be for the outer most road, not the final edge of buildings
        cityGrid = _cleanUpEdges(cityGrid, subsectionWidth, subsectionHeight, out newEdgeVert, out newEdgeHoriz);

        // Place parks
        for (int i = 0; i < _numberOfParks; ++i) {
            cityGrid = _placePark(cityGrid, newEdgeHoriz, newEdgeVert);
        }

        // Randomly remove some roads
        cityGrid = _removeRandomRoads(cityGrid, numberOfSubsectionsUp, numberOfSubsectionsAcross, newEdgeHoriz, newEdgeVert);

		return cityGrid;
    }

    private FILLTYPE[,] _placePark(FILLTYPE[,] cityGrid, int edgeHoriz, int edgeVert) {
        // Pick a random point, then walk until it is a building
        // Then work up and down, left and right replacing building with parks
        int x, y;
        x = Random.Range(2, edgeHoriz - 2);  // don't want the outer edge of buildings or the inside ring of 
        y = Random.Range(2, edgeVert - 2);

        if (cityGrid[y, x] == FILLTYPE.PARK) // just pick another spot
            return _placePark(cityGrid, edgeHoriz, edgeVert);

        // We should never need more than 3 steps to get off a road
        if (cityGrid[y, x] == FILLTYPE.ROAD) {
            ++y;
            if (cityGrid[y, x] == FILLTYPE.ROAD) {
                ++x;
                if (cityGrid[y, x] == FILLTYPE.ROAD) {
                    ++y;
                }
            }
        }

        // At this point, our point should be within a section of buildings
        // Now we want to walk down to the bottom left of the section
        while (cityGrid[y + 1, x] == FILLTYPE.BULIDING) {
            ++y;
        }
        while (cityGrid[y, x - 1] == FILLTYPE.BULIDING) {
            --x;
        }

        // Fill in park
        int tempY = y;
        for (int i = 0; i < _parkFootprintWidth; ++i) {
            y = tempY;
            for (int j = 0; j < _parkFootprintHeight && y > 0; ++j) {
                cityGrid[y--, x] = FILLTYPE.PARK;
            }
            ++x;
        }

        return cityGrid;
    }

    private FILLTYPE[,] _cleanUpEdges(FILLTYPE[,] cityGrid, int subsectionWidth, int subsectionHeight, out int newEdgeVert, out int newEdgeHoriz) {
        int width, height;

        // Eliminate extras along the edges (either overwrite with buildings, or close them with roads if they are large enough)
        width = _cityWidth - 2; // _cityWidth - 1 must be a building, this is the first place we can have a road
        height = 1;  // top layer
        // Walk back until we find the most recent intersection
        do {
            --width;
        } while (cityGrid[height + 1, width] != FILLTYPE.ROAD);
        newEdgeHoriz = width;
        // This is small, so we really just need to cut off the edges.
        ++width;
        for (; width < _cityWidth - 1; ++width) {
            for (; height < _cityHeight - 1; ++height) {
                cityGrid[height, width] = FILLTYPE.BULIDING;
            }
        }

        width = 1;
        height = _cityHeight - 2;
        do {
            --height;
        } while (cityGrid[height, width + 1] != FILLTYPE.ROAD);
        newEdgeVert = height;
        // Check how large the space we have is left
        // If it is enough of the subsection, close it off, otherwise eliminate the extra road
        if ((_cityHeight - 2) - height > subsectionHeight * 0.75) {
            // Close it off
            height = _cityHeight - 2;
            for (; width <= newEdgeHoriz; ++width) {
                cityGrid[height, width] = FILLTYPE.ROAD;
            }
        } else {
            for (; width <= newEdgeHoriz; ++width) {
                for (height = newEdgeVert + 1; height < _cityHeight - 1; ++height) {
                    cityGrid[height, width] = FILLTYPE.BULIDING;
                }
            }
        }

        return cityGrid;

    }

    private FILLTYPE[,] _removeRandomRoads(FILLTYPE[,] cityGrid, int subsectionsVert, int subsectionsHoriz, int horizEdge, int vertEdge) {
        // Pick a couple of roads and simply remove them
        // to add some diversity to the city

        Vector2 pos;
        int verticalRoadsCut, horizontalRoadsCut, x, y;
        verticalRoadsCut = (int)Random.Range(1f, Mathf.Floor(subsectionsHoriz / 2));
        horizontalRoadsCut = (int)Random.Range(1f, Mathf.Floor(subsectionsVert / 2));

        for (int i = 0; i < verticalRoadsCut; ++i) {
            pos = _getCoordinatesOfRoadToRemove(cityGrid, true, horizEdge, vertEdge);
            x = (int)pos.x;
            y = (int)pos.y;

            while (cityGrid[y, x - 1] != FILLTYPE.ROAD && cityGrid[y, x + 1] != FILLTYPE.ROAD) {
                cityGrid[y++, x] = FILLTYPE.BULIDING;
            }
        }

        for (int i = 0; i < horizontalRoadsCut; ++i) {
            pos = _getCoordinatesOfRoadToRemove(cityGrid, false, horizEdge, vertEdge);
            x = (int)pos.x;
            y = (int)pos.y;

            while (cityGrid[y - 1, x] != FILLTYPE.ROAD && cityGrid[y + 1, x] != FILLTYPE.ROAD) {
                cityGrid[y, x++] = FILLTYPE.BULIDING;
            }
        }

        return cityGrid;
    }

    private Vector2 _getCoordinatesOfRoadToRemove(FILLTYPE[,] cityGrid, bool verticalRoad, int horizEdge, int vertEdge) {

        Vector2 pos = new Vector2();
        int x, y;

        if (verticalRoad) {
            x = Random.Range(1, horizEdge - 1);
            y = Random.Range(2, vertEdge - 1);

            // Walk until we are just below a horizontal road
            while (y >= 3 && ((x != 1 && cityGrid[y - 1, x - 1] != FILLTYPE.ROAD) || cityGrid[y - 1, x + 1] != FILLTYPE.ROAD)) {
                --y;
            }

            // Walk until we find a road
            while (x <= horizEdge && cityGrid[y, x] != FILLTYPE.ROAD) {
                ++x;
            }

            if (x > horizEdge) {
                // We already removed the road in this section
                return _getCoordinatesOfRoadToRemove(cityGrid, verticalRoad, horizEdge, vertEdge);
            }

            // Otherwise, we are at a good point, return it
            pos.x = x;
            pos.y = y;

            return pos;
        } else {
            x = Random.Range(2, horizEdge - 1);
            y = Random.Range(1, vertEdge - 1);

            // Walk until we are just to the right of a vertical road
            while (x >= 3 && ((y != 1 && cityGrid[y - 1, x - 1] != FILLTYPE.ROAD) || cityGrid[y + 1, x - 1] != FILLTYPE.ROAD)) {
                --x;
            }

            // Walk until we find a road
            while (y <= vertEdge && cityGrid[y, x] != FILLTYPE.ROAD) {
                ++y;
            }

            if (y > vertEdge) {
                // Road here is already gone
                return _getCoordinatesOfRoadToRemove(cityGrid, verticalRoad, horizEdge, vertEdge);
            }

            pos.x = x;
            pos.y = y;

            return pos;
        }
    }
}
