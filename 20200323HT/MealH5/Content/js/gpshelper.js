
// 获取当前地理位置坐标
function getGps(callback) {
    
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (pos) {
            callback({ state: 1, geo: { lat: pos.coords.latitude , lon: pos.coords.longitude} });
        }, function (err) {
            //callback({ state: 0, err: err });
            setTimeout(function () {
                callback({ state: 1, geo: { lat: 38.860832, lon: 121.518632 } });
            }, 500);
            
        }, {
            enableHighAccuracy: true,
            timeout: 5000,  
            maximumAge: 0
        });
    } else {
        callback({state: -1, txt : '您的浏览器不支持提供GPS位置信息'});
    }

}

// GSP转高德坐标系
function AMapConvert(lon, lat, callback) {
    var amapgps = {};
    AMap.convertFrom(new AMap.LngLat(lon, lat), 'gps', function (state, result) {
        if (state == 'complete') {
            // 得到高德坐标
            var _lon = result.locations[0].getLng();
            var _lat = result.locations[0].getLat();

            amapgps.lon = _lon;
            amapgps.lat = _lat;

            AMap.service('AMap.Geocoder', function () {//回调函数
                var geocoder = new AMap.Geocoder({});

                var lnglatXY = [_lon, _lat];
                geocoder.getAddress(lnglatXY, function (status, result) {
                    if (status === 'complete' && result.info === 'OK') {
                        // 得到当前未知所属行政区
                        amapgps.currentCity = result.regeocode.addressComponent.city;
                        amapgps.currentCityCode = result.regeocode.addressComponent.citycode;
                        amapgps.formattedAddress = result.regeocode.formattedAddress;
                        amapgps.state = 1;
                        callback(amapgps);
                    } else {
                        amapgps.state = 1;
                        amapgps.txt = '获取坐标失败';
                        callback(amapgps);
                    }
                });
            });

        } else {
            amapgps.state = 1;
            amapgps.txt = '获取坐标失败';
            callback(amapgps);
        }
    });
}

// 获取当前位置的高德坐标
function getAMapGeo(callback) {
    var geo = {};
    getGps(function (gpsdata) {
        if (gpsdata.state == 1) {
            geo.lon = gpsdata.geo.lon;
            geo.lat = gpsdata.geo.lat;
            geo.state = 1;
            AMapConvert(gpsdata.geo.lon, gpsdata.geo.lat, function (amapdata) {
                geo.amap = amapdata;
                callback(geo);
            });
        } else {
            callback({state : 0, txt : '获取坐标失败'});
        }
    });
}

