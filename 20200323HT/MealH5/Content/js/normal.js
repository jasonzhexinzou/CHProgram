
var oneDay = 24 * 60 * 60 * 1000;
var eightHours = 8 * 60 * 60 * 1000;
var oneHours = 60 * 60 * 1000;


/** * 对Date的扩展，将 Date 转化为指定格式的String * 月(M)、日(d)、12小时(h)、24小时(H)、分(m)、秒(s)、周(E)、季度(q)
 可以用 1-2 个占位符 * 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) * eg: * (new
 Date()).pattern("yyyy-MM-dd hh:mm:ss.S")==> 2006-07-02 08:09:04.423      
 * (new Date()).pattern("yyyy-MM-dd E HH:mm:ss") ==> 2009-03-10 二 20:09:04      
 * (new Date()).pattern("yyyy-MM-dd EE hh:mm:ss") ==> 2009-03-10 周二 08:09:04      
 * (new Date()).pattern("yyyy-MM-dd EEE hh:mm:ss") ==> 2009-03-10 星期二 08:09:04      
 * (new Date()).pattern("yyyy-M-d h:m:s.S") ==> 2006-7-2 8:9:4.18      
 */
Date.prototype.pattern = function(fmt) {
	var o = {
		"M+" : this.getMonth() + 1, //月份         
		"d+" : this.getDate(), //日         
		"h+" : this.getHours() % 12 == 0 ? 12 : this.getHours() % 12, //小时         
		"H+" : this.getHours(), //小时         
		"m+" : this.getMinutes(), //分         
		"s+" : this.getSeconds(), //秒         
		"q+" : Math.floor((this.getMonth() + 3) / 3), //季度         
		"S" : this.getMilliseconds()
	//毫秒         
	};
	var week = {
	    "1": "星期一",
		"2": "星期二",
		"3": "星期三",
		"4": "星期四",
		"5": "星期五",
		"6": "星期六",
		"0": "星期日"
	};
	if (/(y+)/.test(fmt)) {
		fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "")
				.substr(4 - RegExp.$1.length));
	}
	if (/(E+)/.test(fmt)) {
		fmt = fmt
				.replace(
						RegExp.$1,
						((RegExp.$1.length > 1) ? (RegExp.$1.length > 2 ? "/u661f/u671f"
								: "/u5468")
								: "")
								+ week[this.getDay() + ""]);
	}
	for ( var k in o) {
		if (new RegExp("(" + k + ")").test(fmt)) {
			fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k])
					: (("00" + o[k]).substr(("" + o[k]).length)));
		}
	}
	return fmt;
};

Date.prototype.toString = Date.prototype.pattern;

Date.prototype.parse = function (txt) {
    txt = txt.replace('-', '/');
    
    return new Date(txt);
}

/**
 * 判断参数日期是否从左到右升序
 * @param date1
 * @param date2
 */
function isDateAscending(date1, date2) {
	var d1 = new Date(date1.replace('-', '/'));
	var d2 = new Date(date2.replace('-', '/'));
	return d1.getTime() < d2.getTime();
}

function isDateAscending2(date1, date2) {
    var d1 = new Date(date1.replace('-', '/'));
    var d2 = new Date(date2.replace('-', '/'));
    return d1.getTime() <= d2.getTime();
}

// 判断给定的时间是否在一个时间区间内(默认为当前日期的时间区间)
function isInTimespan(nowTime, beginTime, endTime) {
    var time = nowTime.pattern('HH:mm:ss');

    return isTimeInTimespan(time, beginTime, endTime);
}

// 判断给定的时间是否在一个时间区间内(默认为当前日期的时间区间)
function isTimeInTimespan(nowTime, beginTime, endTime) {
    var time = nowTime;

    var intNow = time.replace(/:/g, '') * 1;
    var intBegin = beginTime.replace(/:/g, '') * 1;
    var intEnd = endTime.replace(/:/g, '') * 1;

    return (intBegin <= intNow) && (intNow <= intEnd);
}

function timeAdd(time, add) {
    var _time = time.split(':');
    var _add = add.split(':');

    for (var i in _time) {
        _time[i] = _time[i] * 1;
    }
    for (var i in _add) {
        _add[i] = _add[i] * 1;
    }

    var _s = _time[2] + _add[2];
    _time[2] = _s % 60;
    _time[1] += parseInt(_s / 60);

    var _m = _time[1] + _add[1];
    _time[1] = _m % 60;
    _time[0] += parseInt(_m / 60);

    _time[0] += _add[0];

    var res = '';
    if (_time[0] < 10) {
        res += '0'; 
    }
    res += _time[0] + ':';

    if (_time[1] < 10) {
        res += '0';
    }
    res += _time[1] + ':';

    if (_time[2] < 10) {
        res += '0';
    }
    res += _time[2];

    return res;
}

/*
 * MAP对象，实现MAP功能
 *
 * 接口：
 * size()     获取MAP元素个数
 * isEmpty()    判断MAP是否为空
 * clear()     删除MAP所有元素
 * put(key, value)   向MAP中增加元素（key, value) 
 * remove(key)    删除指定KEY的元素，成功返回True，失败返回False
 * get(key)    获取指定KEY的元素值VALUE，失败返回NULL
 * element(index)   获取指定索引的元素（使用element.key，element.value获取KEY和VALUE），失败返回NULL
 * containsKey(key)  判断MAP中是否含有指定KEY的元素
 * containsValue(value) 判断MAP中是否含有指定VALUE的元素
 * values()    获取MAP中所有VALUE的数组（ARRAY）
 * keys()     获取MAP中所有KEY的数组（ARRAY）
 *
 * 例子：
 * var map = new Map();
 *
 * map.put("key", "value");
 * var val = map.get("key")
 * ……
 *
 */
function Map() {
    this.elements = new Array();

    //获取MAP元素个数
    this.size = function() {
        return this.elements.length;
    };

    //判断MAP是否为空
    this.isEmpty = function() {
        return (this.elements.length < 1);
    };

    //删除MAP所有元素
    this.clear = function() {
        this.elements = new Array();
    };

    //向MAP中增加元素（key, value) 
    this.put = function(_key, _value) {
        this.elements.push( {
            key : _key,
            value : _value
        });
    };

    //删除指定KEY的元素，成功返回True，失败返回False
    this.remove = function(_key) {
        var bln = false;
        try {
            for (i = 0; i < this.elements.length; i++) {
                if (this.elements[i].key == _key) {
                    this.elements.splice(i, 1);
                    return true;
                }
            }
        } catch (e) {
            bln = false;
        }
        return bln;
    };

    //获取指定KEY的元素值VALUE，失败返回NULL
    this.get = function(_key) {
        try {
            for (i = 0; i < this.elements.length; i++) {
                if (this.elements[i].key == _key) {
                    return this.elements[i].value;
                }
            }
        } catch (e) {
            return null;
        }
    };

    //获取指定索引的元素（使用element.key，element.value获取KEY和VALUE），失败返回NULL
    this.element = function(_index) {
        if (_index < 0 || _index >= this.elements.length) {
            return null;
        }
        return this.elements[_index];
    };

    //判断MAP中是否含有指定KEY的元素
    this.containsKey = function(_key) {
        var bln = false;
        try {
            for (i = 0; i < this.elements.length; i++) {
                if (this.elements[i].key == _key) {
                    bln = true;
                }
            }
        } catch (e) {
            bln = false;
        }
        return bln;
    };

    //判断MAP中是否含有指定VALUE的元素
    this.containsValue = function(_value) {
        var bln = false;
        try {
            for (i = 0; i < this.elements.length; i++) {
                if (this.elements[i].value == _value) {
                    bln = true;
                }
            }
        } catch (e) {
            bln = false;
        }
        return bln;
    };

    //获取MAP中所有VALUE的数组（ARRAY）
    this.values = function() {
        var arr = new Array();
        for (i = 0; i < this.elements.length; i++) {
            arr.push(this.elements[i].value);
        }
        return arr;
    };

    //获取MAP中所有KEY的数组（ARRAY）
    this.keys = function() {
        var arr = new Array();
        for (i = 0; i < this.elements.length; i++) {
            arr.push(this.elements[i].key);
        }
        return arr;
    };
};

/*************** 一些通 DataGrid 通用的format方法 *****************************/

/**
 * 把.Net输出的 /Date(1445421250444)/ 转化为js 的Date格式
 */
function getDateByDotNet(txt) {
    return eval('new ' + (txt.replace(/\//g, '')))
}

/**
 * 日期格式化
 */
function dateFormatter(value) {
	if (value == undefined || value == null || value == '')
		return value;
	return new Date(value).pattern("yyyy-MM-dd");
}

/**
 * 日期时间格式化
 */
function dateTimeFormatter(value) {
	if (value == undefined || value == null || value == '')
		return value;
	return new Date(value).pattern("yyyy-MM-dd HH:mm:ss");
}

/**
 * 日期时间格式化,根据.Net输出的时间
 */
function dateTimeFormatterByDotNet(txt) {
    return dateTimeFormatter(getDateByDotNet(txt));
}

/**
 * 表示是或者否的布尔值格式化
 * @param value
 */
function booleanCNFormatter(value) {
	
	if (value == 1 || value == true) {
		return "是";
	} else {
		return "否";
	}
};

/**
 * 表示有或者无的布尔值格式化
 * @param value
 */
function booleanHaveFormatter(value) {
	
	if (value == 1 || value == true) {
		return "有";
	} else {
		return "无";
	}
};

/**
 * 图片地址格式化
 * @param value
 */
function imgurlFormatter(value) {
	if (value == undefined || value == null || value == '' || value == 'null')
		return "";
	if (value.indexOf(',') == -1) {
		var url = ctx + value;
		return '<a href="' + url + '" target="_blank"><img alt="" src="' + ctx + '/static/images/pic.gif"></a>';
	} else {
		var html = '';
		var urls = value.split(',');
		for (var i in urls) {
			var url = ctx + urls[i];
			html += '<a href="' + url + '" target="_blank"><img alt="" src="' + ctx + '/static/images/pic.gif"></a>\n';
		}
		return html;
	}
	
};

/* 密码由字母和数字组成，至少6位 */
var safePassword = function (value) {
	if (/[@#\$%\^&\*.':;,<>\/?~`\[\]\{\}\\\"|]+/g.test(value)){
		return false;
	}
	return !(/^(([A-Z]*|[a-z]*|\d*|[-_\~!@#\$%\^&\*\.\(\)\[\]\{\}<>\?\\\/\'\"]*)|.{0,5})$|\s/.test(value));
}

var idCard = function (value) {
    if (value.length == 18 && 18 != value.length) return false;
    var number = value.toLowerCase();
    var d, sum = 0, v = '10x98765432', w = [7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2], a = '11,12,13,14,15,21,22,23,31,32,33,34,35,36,37,41,42,43,44,45,46,50,51,52,53,54,61,62,63,64,65,71,81,82,91';
    var re = number.match(/^(\d{2})\d{4}(((\d{2})(\d{2})(\d{2})(\d{3}))|((\d{4})(\d{2})(\d{2})(\d{3}[x\d])))$/);
    if (re == null || a.indexOf(re[1]) < 0) return false;
    if (re[2].length == 9) {
        number = number.substr(0, 6) + '19' + number.substr(6);
        d = ['19' + re[4], re[5], re[6]].join('-');
    } else d = [re[9], re[10], re[11]].join('-');
    if (!isDateTime.call(d, 'yyyy-MM-dd')) return false;
    for (var i = 0; i < 17; i++) sum += number.charAt(i) * w[i];
    return (re[2].length == 9 || number.charAt(17) == v.charAt(sum % 11));
}

var isDateTime = function (format, reObj) {
    format = format || 'yyyy-MM-dd';
    var input = this, o = {}, d = new Date();
    var f1 = format.split(/[^a-z]+/gi), f2 = input.split(/\D+/g), f3 = format.split(/[a-z]+/gi), f4 = input.split(/\d+/g);
    var len = f1.length, len1 = f3.length;
    if (len != f2.length || len1 != f4.length) return false;
    for (var i = 0; i < len1; i++) if (f3[i] != f4[i]) return false;
    for (var i = 0; i < len; i++) o[f1[i]] = f2[i];
    o.yyyy = s(o.yyyy, o.yy, d.getFullYear(), 9999, 4);
    o.MM = s(o.MM, o.M, d.getMonth() + 1, 12);
    o.dd = s(o.dd, o.d, d.getDate(), 31);
    o.hh = s(o.hh, o.h, d.getHours(), 24);
    o.mm = s(o.mm, o.m, d.getMinutes());
    o.ss = s(o.ss, o.s, d.getSeconds());
    o.ms = s(o.ms, o.ms, d.getMilliseconds(), 999, 3);
    if (o.yyyy + o.MM + o.dd + o.hh + o.mm + o.ss + o.ms < 0) return false;
    if (o.yyyy < 100) o.yyyy += (o.yyyy > 30 ? 1900 : 2000);
    d = new Date(o.yyyy, o.MM - 1, o.dd, o.hh, o.mm, o.ss, o.ms);
    var reVal = d.getFullYear() == o.yyyy && d.getMonth() + 1 == o.MM && d.getDate() == o.dd && d.getHours() == o.hh && d.getMinutes() == o.mm && d.getSeconds() == o.ss && d.getMilliseconds() == o.ms;
    return reVal && reObj ? d : reVal;
    function s(s1, s2, s3, s4, s5) {
        s4 = s4 || 60, s5 = s5 || 2;
        var reVal = s3;
        if (s1 != undefined && s1 != '' || !isNaN(s1)) reVal = s1 * 1;
        if (s2 != undefined && s2 != '' && !isNaN(s2)) reVal = s2 * 1;
        return (reVal == s1 && s1.length != s5 || reVal > s4) ? -10000 : reVal;
    }
};

/**
 * HTML编码
 * @param str
 * @returns {String}
 */
function HTMLEnCode(str) {
	var s = "";
	if (str.length == 0)
		return "";
	s = str.replace(/&/g, "&gt;");
	s = s.replace(/</g, "&lt;");
	s = s.replace(/>/g, "&gt;");
	s = s.replace(/    /g, "&nbsp;");
	s = s.replace(/\'/g, "'");
	s = s.replace(/\"/g, "&quot;");
	s = s.replace(/\n/g, "<br>");
	return s;
}

/**
 * HTML解码
 * @param str
 * @returns {String}
 */
function HTMLDeCode(str) {
	var s = "";
	if (str.length == 0)
		return "";
	s = str.replace(/&amp;/g, "&");
	s = s.replace(/&lt;/g, "<");
	s = s.replace(/&gt;/g, ">");
	s = s.replace(/&nbsp;/g, "    ");
	s = s.replace(/'/g, "\'");
	s = s.replace(/&quot;/g, "\"");
	s = s.replace(/<br>/g, "\n");
	s = s.replace(/&#39;/g, "\'");
	return s;
}

/**
 * 获取缩略图路径
 */
function GetThumbnailsUri(uri) {
    var _imgUri = uri.substring(0, uri.lastIndexOf('.'));
    _imgUri += '_0' + uri.substr(uri.lastIndexOf('.'));
    return _imgUri;
}

// 获取字符串长度，英文1，中文2
var getStirngANSIILength = function (str) {
    var realLength = 0, len = str.length, charCode = -1;
    for (var i = 0; i < len; i++) {
        charCode = str.charCodeAt(i);
        if (charCode >= 0 && charCode <= 128) realLength += 1;
        else realLength += 2;
    }
    return realLength;
};

/**
 * 获取文件路径中的扩展名
 */
function getFileExtName(filename) {
    var index1 = filename.lastIndexOf(".");
    var index2 = filename.length;
    var postf = filename.substring(index1, index2);//后缀名  
    return postf;
}

function UrlEncode(str) {
    str = (str + '').toString();
    return encodeURIComponent(str).replace(/!/g, '%21').replace(/'/g, '%27').replace(/\(/g, '%28').replace(/\)/g, '%29').replace(/\*/g, '%2A').replace(/%20/g, '+');
}


function toTowFloat(value) {
    value = Number(value);
    var num = '' + value.toFixed(2);
    for (var i = 0; i < 2; i++) {
        if (num.charAt(num.length - 1) == '0') {
            num = num.substring(0, num.length - 1);
        }
    }
    if (num.charAt(num.length - 1) == '.') {
        num = num.substring(0, num.length - 1);
    }
    return Number(num);
}

