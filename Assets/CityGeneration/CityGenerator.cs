using UnityEngine;
using System.Collections;

public class CityGenerator : MonoBehaviour {

    /*
     * Control variables
     */

    // width and height of city
    private int _cityWidth, _cityHeight;
    // width and height (2d) of a building
    private int _buildingFootprintWidth, _buildingFootprintHeight;
    // width of road
    private int _roadWidth;
    // width and height of park
    private int _parkFootprintWidth, _parkFootprintHeight;
    // number of parks in city
    private int _numberOfParks;

    /*
     * Enum for identifying areas
     */

    private enum FILLTYPE {
        EMPTY = 0,
        BULIDING,
        ROAD,
        PARK
    };
 
    /*
     * Starter functions
     */

    public void buildCity() {
        _buildCity();
    }

    public void buildCity(int cityWidth, int cityHeight) {
        _cityWidth = cityWidth;
        _cityHeight = cityHeight;
        _buildCity();
    }

    public void buildCity(int cityWidth, int cityHeight, int buildingWidth, int buildingHeight) {
        _cityWidth = cityWidth;
        _cityHeight = cityHeight;
        _buildingFootprintWidth = buildingWidth;
        _buildingFootprintHeight = buildingHeight;
        _buildCity();
    }

    public void buildCity(int cityWidth, int cityHeight, int buildingWidth, int buildingHeight, int roadWidth) {
        _cityWidth = cityWidth;
        _cityHeight = cityHeight;
        _buildingFootprintWidth = buildingWidth;
        _buildingFootprintHeight = buildingHeight;
        _roadWidth = roadWidth;
        _buildCity();
    }

    public void buildCity(int cityWidth, int cityHeight, int buildingWidth, int buildingHeight, int roadWidth, int parkWidth, int parkHeight, int parkCount) {
        _cityWidth = cityWidth;
        _cityHeight = cityHeight;
        _buildingFootprintWidth = buildingWidth;
        _buildingFootprintHeight = buildingHeight;
        _roadWidth = roadWidth;
        _parkFootprintWidth = parkWidth;
        _parkFootprintHeight = parkHeight;
        _numberOfParks = parkCount;
        _buildCity();
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

    public void setBuildingFootprint(int width, int height) {
        _buildingFootprintWidth = width;
        _buildingFootprintHeight = height;
    }

    public int getBuildingFootprintWidth() {
        return _buildingFootprintWidth;
    }

    public int getBuildingFootprintHeight() {
        return _buildingFootprintHeight;
    }

    public void setRoadWidth(int width) {
        _roadWidth = width;
    }

    public int getRoadWidth() {
        return _roadWidth;
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

    private void _buildCity() {

    }
}
