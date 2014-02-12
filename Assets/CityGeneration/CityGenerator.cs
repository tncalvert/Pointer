using UnityEngine;
using System.Collections;
using System.IO;

public class CityGenerator : MonoBehaviour {

    // ** For testing **
    public void Start() {
        Debug.Log("Testing city generation");
        buildCity();
    }

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
    private int _parkFootprintWidth = 2, _parkFootprintHeight = 2;
    // number of parks in city
    private int _numberOfParks = 1;

    /*
     * Enum for identifying areas
     */

    private enum FILLTYPE {
        BULIDING = 0,
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

    public void buildCity(int cityWidth, int cityHeight,int parkWidth, int parkHeight, int parkCount) {
        _cityWidth = cityWidth;
        _cityHeight = cityHeight;
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
        height = 6;
        for (int i = 0; i < numberOfSubsectionsUp; ++i) {
            for (width = 1; width < _cityWidth - 1; ++width) {
                cityGrid[height, width] = FILLTYPE.ROAD;
            }

            height += subsectionHeight;
        }

        // TODO: Place parks

        // DEBUG
        Texture2D debugTexture = new Texture2D(_cityWidth, _cityHeight);
        for (width = 0; width < _cityWidth; ++width) {
            for (height = 0; height < _cityHeight; ++height) {

                switch (cityGrid[height, width]) {

                    case FILLTYPE.BULIDING:
                        debugTexture.SetPixel(width, height, Color.blue);
                        break;
                    case FILLTYPE.ROAD:
                        debugTexture.SetPixel(width, height, Color.grey);
                        break;
                    case FILLTYPE.PARK:
                        debugTexture.SetPixel(width, height, Color.green);
                        break;

                }

            }
        }
        debugTexture.Apply();
        File.WriteAllBytes(Application.dataPath + "/CityTexture.png", debugTexture.EncodeToPNG());
    }
}
