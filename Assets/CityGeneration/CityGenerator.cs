using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

    /*
     * Enum for identifying areas
     */

    public enum FILLTYPE {
        BULIDING = 0,
        ROAD,
        PARK
    };

    /*
     * Enum for flood fill
     */

    private enum FLOODFILL {
        IGNORE = 0,
        REACHABLE,
        UNREACHABLE
    }
 
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

    public FILLTYPE[,] buildCity(int cityWidth, int cityHeight, int parkCount) {
        _cityWidth = cityWidth;
        _cityHeight = cityHeight;
        _numberOfParks = parkCount;
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

        // Make sure all sections are connected
        cityGrid = _fixCutoffSections(cityGrid, newEdgeHoriz, newEdgeVert);

		return cityGrid;
    }

    private FILLTYPE[,] _placePark(FILLTYPE[,] cityGrid, int edgeHoriz, int edgeVert) {
        // Pick a random point, then walk until it is a building
        // Then work up and down, left and right replacing building with parks
        int x, y;
        int attemptCount = 0, maxAttempts = 15;

        do {
            ++attemptCount;
            if (attemptCount >= maxAttempts) {
                return cityGrid;
            }

            x = Random.Range(2, edgeHoriz - 2);  // don't want the outer edge of buildings or the inside ring of 
            y = Random.Range(2, edgeVert - 2);

        } while (cityGrid[y, x] == FILLTYPE.PARK);

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

        
        int totalIntersections = subsectionsHoriz * subsectionsVert;
        Vector2 pos;
        int verticalRoadsCut, horizontalRoadsCut, x, y;
        verticalRoadsCut = (int)Random.Range(Mathf.Max(1f, Mathf.Floor(totalIntersections / 4)),
            Mathf.Floor(totalIntersections / 2));
        horizontalRoadsCut = (int)Random.Range(Mathf.Max(1f, Mathf.Floor(totalIntersections / 4)),
            Mathf.Floor(totalIntersections / 2));

        for (int i = 0; i < verticalRoadsCut; ++i) {
            pos = _getCoordinatesOfRoadToRemove(cityGrid, true, horizEdge, vertEdge);

            if (pos.x == -1 && pos.y == -1) {
                continue;
            }

            x = (int)pos.x;
            y = (int)pos.y;

            while (y <= vertEdge && cityGrid[y, x - 1] != FILLTYPE.ROAD && cityGrid[y, x + 1] != FILLTYPE.ROAD) {
                cityGrid[y++, x] = FILLTYPE.BULIDING;
            }
        }

        for (int i = 0; i < horizontalRoadsCut; ++i) {
            pos = _getCoordinatesOfRoadToRemove(cityGrid, false, horizEdge, vertEdge);

            if (pos.x == -1 && pos.y == -1) {
                continue;
            }

            x = (int)pos.x;
            y = (int)pos.y;
            
            while (x <= horizEdge && cityGrid[y - 1, x] != FILLTYPE.ROAD && cityGrid[y + 1, x] != FILLTYPE.ROAD) {
                cityGrid[y, x++] = FILLTYPE.BULIDING;
            }
        }

        return cityGrid;
    }

    private Vector2 _getCoordinatesOfRoadToRemove(FILLTYPE[,] cityGrid, bool verticalRoad, int horizEdge, int vertEdge) {

        Vector2 pos = new Vector2();
        int x, y;
        int attemptCount = 0, maxAttempts = 15;

        if (verticalRoad) {
            do {

                ++attemptCount;
                if (attemptCount >= maxAttempts) {
                    pos.x = -1;
                    pos.y = -1;
                    return pos;
                }

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

            // If x is out of bounds, try again
            } while (x > horizEdge);

            // Otherwise, we are at a good point, return it
            pos.x = x;
            pos.y = y;

            return pos;
        } else {
            do {

                ++attemptCount;
                if (attemptCount >= maxAttempts) {
                    pos.x = -1;
                    pos.y = -1;
                    return pos;
                }

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

            } while (y > vertEdge);

            pos.x = x;
            pos.y = y;

            return pos;
        }
    }

    private FILLTYPE[,] _fixCutoffSections(FILLTYPE[,] cityGrid, int edgeHoriz, int edgeVert) {

        FLOODFILL[,] floodGrid = new FLOODFILL[_cityHeight, _cityWidth];
        int initX, initY, unreachablePoints;
        
       	do {
            initX = -1;
            initY = -1;
            unreachablePoints = 0;

            for (int i = 0; i < _cityHeight; ++i) {
                for (int j = 0; j < _cityWidth; ++j) {

                    if (cityGrid[i, j] == FILLTYPE.ROAD) {
                        floodGrid[i, j] = FLOODFILL.UNREACHABLE;
                        if (initX == -1 && initY == -1) {
                            initX = j;
                            initY = i;
                        }
                    } else {
                        floodGrid[i, j] = FLOODFILL.IGNORE;
                    }
                }
            }

            if (initX == -1 && initY == -1) {
                // Something when terribly wrong
                return cityGrid;
            }

            // Flood fill
            floodGrid = _floodFill(floodGrid, new Vector2(initX, initY));

			bool breakLoop = false;
            // Check how many points are unreachable and try to fix them
            for (int i = 0; i < _cityHeight; ++i) {
                for (int j = 0; j < _cityWidth; ++j) {
                    if (floodGrid[i, j] == FLOODFILL.UNREACHABLE) {
						++unreachablePoints;
                        List<Vector2> path = _getShortestPathToOpenRoad(floodGrid, new Vector2(j, i), cityGrid);

                        if (path.Count == 0) {
                            continue;
                        }

                        cityGrid = _cutPath(cityGrid, path);

						breakLoop = true;
						break;
                    }
                }
				if(breakLoop) { break; }
            }

        } while (unreachablePoints != 0);


        return cityGrid;
    }

    private FLOODFILL[,] _floodFill(FLOODFILL[,] floodGrid, Vector2 pos) {

        Queue<Vector2> q = new Queue<Vector2>();

        if (floodGrid[(int)pos.y, (int)pos.x] == FLOODFILL.IGNORE) {
            return floodGrid;
        }

        q.Enqueue(pos);

        while(q.Count != 0) {
            Vector2 p = q.Dequeue();
            Vector2 w, e;

            if (floodGrid[(int)p.y, (int)p.x] == FLOODFILL.UNREACHABLE) {
                w = new Vector2(p.x - 1, p.y);
                e = new Vector2(p.x + 1, p.y);

                while (floodGrid[(int)e.y, (int)e.x] == FLOODFILL.UNREACHABLE) {
                    e.x += 1;
                }

                while (floodGrid[(int)w.y, (int)w.x] == FLOODFILL.UNREACHABLE) {
                    w.x -= 1;
                }

                // The two x value will point to the first IGNORE or REACHABLE value they come upon
                int left = (int)w.x + 1;
                int right = (int)e.x - 1;
                int y = (int)e.y;

                for (; left <= right; ++left) {
                    floodGrid[y, left] = FLOODFILL.REACHABLE;
                    if (floodGrid[y - 1, left] == FLOODFILL.UNREACHABLE) {
                        q.Enqueue(new Vector2(left, y - 1));
                    }
                    if (floodGrid[y + 1, left] == FLOODFILL.UNREACHABLE) {
                        q.Enqueue(new Vector2(left, y + 1));
                    }
                }
            }
        }

        return floodGrid;
    }

    private List<Vector2> _getShortestPathToOpenRoad(FLOODFILL[,] floodGrid, Vector2 initPoint, FILLTYPE[,] cityGrid) {
        List<Vector2> visitedNodes = new List<Vector2>();
        List<BFSStorage> fringe = new List<BFSStorage>();
        BFSStorage currentState = new BFSStorage(initPoint, null);
        int x = (int)currentState.nodeLocation.x, y = (int)currentState.nodeLocation.y;

        while ((y > 0 && y < _cityHeight - 1) && (x > 0 && x < _cityWidth - 1) && floodGrid[y, x] != FLOODFILL.REACHABLE) {

            // Add successors, making sure to not go off the edge or through parks
            if (y - 1 > 0) {
                if (cityGrid[y - 1, x] != FILLTYPE.PARK) {
                fringe.Add(new BFSStorage(new Vector2(x, y - 1), currentState));
                }
            }
            if (y + 1 < _cityHeight - 1) {
                if (cityGrid[y + 1, x] != FILLTYPE.PARK) {
					fringe.Add(new BFSStorage(new Vector2(x, y + 1), currentState));
                }
            }
            if (x - 1 > 0) {
                if (cityGrid[y, x - 1] != FILLTYPE.PARK) {
					fringe.Add(new BFSStorage(new Vector2(x - 1, y), currentState));
                }
            }
            if (x + 1 < _cityWidth - 1) {
                if (cityGrid[y, x + 1] != FILLTYPE.PARK) {
					fringe.Add(new BFSStorage(new Vector2(x + 1, y), currentState));
                }
            }

            visitedNodes.Add(currentState.nodeLocation);

            if (fringe.Count == 0) {
                // No path
                return new List<Vector2>();
            }

            // Get the next node
            do {
                currentState = fringe[0];
				fringe.RemoveAt(0);
                x = (int)currentState.nodeLocation.x;
                y = (int)currentState.nodeLocation.y;
            } while (visitedNodes.Contains(currentState.nodeLocation));
        }

        // Found a solution, get the path
        List<Vector2> directions = new List<Vector2>();
        while (currentState.parentNode != null) {
            directions.Insert(0, currentState.nodeLocation);
            currentState = currentState.parentNode;
        }

        return directions;
    }

    private class BFSStorage {

		public BFSStorage() {
			nodeLocation = new Vector2();
			parentNode = null;
		}

        public BFSStorage(Vector2 nLocation, BFSStorage pNode) {
            nodeLocation = nLocation;
            parentNode = pNode;
        }

        public Vector2 nodeLocation;
        public BFSStorage parentNode;

        public override string ToString() {
            string temp = "(" + nodeLocation.x + ", " + nodeLocation.y + ")::";
            if (parentNode == null) {
				temp += "(" + parentNode.nodeLocation.x + ", " + parentNode.nodeLocation.y + ")";
            } else {
                temp += "null";
            }
            return temp;
        }
    }

    private FILLTYPE[,] _cutPath(FILLTYPE[,] cityGrid, List<Vector2> path) {

        foreach (var p in path) {
            cityGrid[(int)p.y, (int)p.x] = FILLTYPE.ROAD;
        }

        return cityGrid;
    }
}
