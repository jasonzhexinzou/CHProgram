var iPath = function () { };
iPath.fn = iPath.prototype;

/** 
 * POST方法
 */
iPath.Post = function (url, data, fn, datatype) {
    if (datatype == undefined) {
        datatype = 'json';
    }
    iPath.LodingMask();
    $.post(url, data, function (d) {
        try {
            fn(d);
        } catch (e) { }
        iPath.UnLodingMask();
    }, datatype);
}

/** 
 * 显示载入中蒙板
 */
iPath.LodingMask = function () {
    if ($('.loadingmask').length < 1) {
        $('body').append('<div class="loadingmask"></div>');
    }
    $('.loadingmask').show();
}

/** 
 * 隐藏载入中蒙板
 */
iPath.UnLodingMask = function () {
    $('.loadingmask').hide();
}

/**
 * 数据绑定
 * template: 数据模版
 * datasource: 数据源(数据结构定义，要求必须是Array类型)
 * varname: 循环变量
 */
iPath.DataBind = function (template, datasource, varname) {
    var rel = '';
    for (var i in datasource) {
        var item = datasource[i];
        item._index = i;
        rel += iPath.LineBind(template, item, varname);
    }
    return rel;
}

/**
 * 逐行绑定
 * template: 数据模版
 * item: 本行数据对象
 * varname: 循环变量
 */
iPath.LineBind = function (template, item, varname) {
    var line = template.toString();
    for (; line.indexOf('${') > -1 ;) {
        var el = line.substring(line.indexOf('${'), line.indexOf('}$', line.indexOf('${')) + 2);
        line = line.replace(el, function () {
            return iPath.ElInterpret(item, el, varname);
        });
    }
    return line;
}
iPath.LineBindNew = function (template, item, varname) {
    var line = template.toString();
    for (var i = 0; i < item.length; i++) {

    }
    for (; line.indexOf('${') > -1;) {
        var el = line.substring(line.indexOf('${'), line.indexOf('}$', line.indexOf('${')) + 2);
        line = line.replace(el, function () {
            var str = el.substring(5, el.length - 2);
            return item.p.substring(item.p.indexOf(str), item.p.indexOf(',', item.p.indexOf(str))).replace(str + '=', '');

            //for (var i = 0; i < item.length; i++) {
            //    if (item[i].Key == str)
            //        return item[i].Value;
            //}
            //return iPath.ElInterpretNew(item, el, varname);
        });
    }
    return line;
}

/**
 * 执行el表达式并反馈执行结果
 * item: 本行数据对象
 * el: 需要执行的el表达式
 * varname: 循环变量
 */
iPath.ElInterpret = function (item, el, varname) {
    el = el.substring(2, el.length - 2);
    if (varname == undefined) {
        return eval(el);
    } else {
        el = 'var ' + varname + ' = item; \n' + el;
        return eval(el);
    }

}

iPath.ElInterpretNew = function (item, el, varname) {
    el = el.substring(2, el.length - 2);
    if (varname == undefined) {
        return eval(el);
    } else {
        el = 'var ' + varname + ' = item; \n' + el;
        return eval(el);
    }

}

iPath.Count = function (data, fn) {
    var rel = 0;
    for (var i in data) {
        var item = data[i];
        if (fn(item)) {
            rel++;
        }
    }
    return rel;
}

iPath.Sum = function (data, fn) {
    var rel = 0;
    for (var i in data) {
        var item = data[i];
        rel += fn(item);
    }
    return rel;
}

iPath.Select = function (data, fn) {
    var rel = new Array();
    for (var i in data) {
        var item = data[i];
        rel.push(fn(item));
    }
    return rel;
}

iPath.Where = function (data, fn) {
    var rel = new Array();
    for (var i in data) {
        var item = data[i];
        var _item = fn(item);
        if (_item != undefined && _item != null) {
            rel.push(_item);
        }
    }
    return rel;
}

var iPathDataBind = function () { };
iPathDataBind.fn = iPathDataBind.prototype;
iPathDataBind.fn.jq_obj = undefined;
iPathDataBind.fn.template = undefined;

jQuery.fn.iPathDataBind = function () {
    var $iPathDataBind = new iPathDataBind();
    $iPathDataBind.jq_obj = this;
    if (this.find('script').length > 0) {
        $iPathDataBind.template = this.find('script:eq(0)').html();
    } else {
        $iPathDataBind.template = this.html();
    }
    this.html('');
    return $iPathDataBind;
};

/**
 * 数据绑定
 * jQDom: jqueryDom对象
 * datasource: 数据源(数据结构定义，要求必须是Array类型)
 */
iPathDataBind.fn.DataBind = function (datasource) {
    var html = iPath.DataBind(this.template, datasource, this.jq_obj.attr('var'));
    this.jq_obj.html(html);
}

/**
 * 数据绑定 追加
 * jQDom: jqueryDom对象
 * datasource: 数据源(数据结构定义，要求必须是Array类型)
 */
iPathDataBind.fn.AppendDataBind = function (datasource) {
    var html = iPath.DataBind(this.template, datasource, this.jq_obj.attr('var'));
    this.jq_obj.append(html);
}


/**
* 从下面开始，是Master.Jiang扩展的拼图数据表格插件
*/
function PagingBar() { };
PagingBar.fn = PagingBar.prototype;
PagingBar.fn.prev = undefined;
PagingBar.fn.page = undefined;
PagingBar.fn.next = undefined;
PagingBar.fn.index = undefined;
PagingBar.fn.go = undefined;

function iPathDataGrid() { };
iPathDataGrid.fn = iPathDataGrid.prototype;
iPathDataGrid.fn.jq_obj = undefined;
iPathDataGrid.fn.total = 0;
iPathDataGrid.fn.rows = 10;
iPathDataGrid.fn.page = 1;
iPathDataGrid.fn.total = 0;
iPathDataGrid.fn.url = '';
iPathDataGrid.fn.paging = false;
iPathDataGrid.fn.pagingBar = undefined;
iPathDataGrid.fn.rowsData = undefined;
iPathDataGrid.fn.QueryParams = function () {
    return {};
};
iPathDataGrid.fn.trTmpl = '';
iPathDataGrid.fn.LineFormatter = function (dr) {
    dr._index = dr.index;
    return iPath.LineBind(this.trTmpl, dr, 'dr');
};
iPathDataGrid.fn.LineFormatterNew = function (dr) {
    dr._index = dr.index;
    return iPath.LineBindNew(this.trTmpl, dr, 'dr');
};
iPathDataGrid.fn.Rendering = function () {

};
iPathDataGrid.fn.LoadComplete = function (dr) {

};
iPathDataGrid.fn.LoadData = function () {
    var postdata = this.QueryParams();
    if (postdata == false)
        return;
    postdata.rows = this.rows;
    postdata.page = this.page;

    var pdg = this;
    window.top.showLoading();
    $.ajax(
        {
            url: this.url,
            type: 'post',
            data: postdata,
            dataType: 'json',
            success: function (data) {
                window.top.hideLoading();
                if (data.state == 1) {
                    pdg.total = data.total;
                    var tbody = pdg.jq_obj.find('tbody');
                    tbody.html('');

                    pdg.rowsData = data.rows;

                    pdg.LoadComplete();

                    var tbodyhtml = '';
                    var index = (pdg.page - 1) * pdg.rows;
                    for (var i in data.rows) {
                        index++;
                        var dr = data.rows[i];
                        dr.index = index;
                        tbodyhtml += pdg.LineFormatter(dr);
                    }
                    tbody.html(tbodyhtml);

                    if (pdg.paging == true) {
                        var maxPage = pdg.total < 1 ? 1 : Math.ceil(pdg.total / pdg.rows);
                        var _pageHtml = '' + pdg.page + '/' + maxPage;
                        pdg.pagingBar.page.html(_pageHtml);
                        pdg.pagingBar.index.val(pdg.page);
                    }
                    pdg.LoadComplete();
                } else if (data.state == 99) {
                    pdg.total = data.total;
                    var tbody = pdg.jq_obj.find('tbody');
                    tbody.html('');

                    pdg.rowsData = data.rows;

                    pdg.LoadComplete();

                    var tbodyhtml = '';
                    var index = (pdg.page - 1) * pdg.rows;
                    for (var i in data.rows) {
                        index++;
                        var dr = data.rows[i];
                        dr.index = index;
                        tbodyhtml += pdg.LineFormatterNew(dr);
                    }
                    tbody.html(tbodyhtml);

                    if (pdg.paging == true) {
                        var maxPage = pdg.total < 1 ? 1 : Math.ceil(pdg.total / pdg.rows);
                        var _pageHtml = '' + pdg.page + '/' + maxPage;
                        pdg.pagingBar.page.html(_pageHtml);
                        pdg.pagingBar.index.val(pdg.page);
                    }
                    pdg.LoadComplete();
                } else {
                    $showdialog({ body: data.txt });
                }
            },
            error: function () {
                window.top.hideLoading();
                $showdialog({ body: '通讯错误' });
            }
        }
    );
};
iPathDataGrid.fn.Load = function () {
    this.page = 1;
    this.LoadData();
};
iPathDataGrid.fn.LoadPage = function (page) {
    var maxPage = this.total < 1 ? 1 : Math.ceil(this.total / this.rows);
    if (maxPage < page || 1 > page) {
        $showdialog({ body: '请输入有效的页码值！' });
        return;
    }
    this.page = page;
    this.LoadData();
};
iPathDataGrid.fn.LoadPrev = function () {
    if (this.page > 1) {
        this.page = this.page - 1;
        this.LoadData();
    }
};
iPathDataGrid.fn.LoadNext = function () {
    var maxPage = this.total < 1 ? 1 : Math.ceil(this.total / this.rows);
    if (maxPage > this.page) {
        this.page = this.page + 1;
        this.LoadData();
    }
};
// 将拼图表格扩展为jQuery的插件
jQuery.fn.iPathDataGrid = function (e) {
    var pdg = new iPathDataGrid();
    pdg.jq_obj = this;
    if (pdg.jq_obj.find('tbody').find('script').length > 0) {
        pdg.trTmpl = pdg.jq_obj.find('tbody').find('script:eq(0)').html();
    } else {
        pdg.trTmpl = pdg.jq_obj.find('tbody').html();
    }

    if (e.rows != undefined) {
        pdg.rows = e.rows
    }
    if (e.page != undefined) {
        pdg.page = e.page
    }
    if (e.paging == true) {
        pdg.paging = true;

        // 放入分页footbar
        var footHtml = '';
        footHtml += '        <div class="form-inline" >\n';
        footHtml += '            <button class="button icon-caret-left paging_prev" type="button"></button>';
        footHtml += '            <label class="paging_page">1/1</label>';
        footHtml += '            <button class="button icon-caret-right paging_next" type="button"></button>';
        footHtml += '            <input type="text" class="input input-auto paging_index" style="width:40px; text-align:center;" />';
        footHtml += '            <button class="button icon-anchor paging_go" type="button">&nbsp;&nbsp;跳转</button>';
        footHtml += '        </div>';

        var $paging;
        if (e.paging_id != undefined) {
            $paging = $('#' + e.paging_id);
        } else {
            $paging = this.find('.paging');
        }

        $paging.html(footHtml);

        var pagingBar = new PagingBar();

        pagingBar.prev = $paging.find('.paging_prev');
        pagingBar.page = $paging.find('.paging_page');
        pagingBar.next = $paging.find('.paging_next');
        pagingBar.index = $paging.find('.paging_index');
        pagingBar.go = $paging.find('.paging_go');

        pagingBar.prev.click(function () {
            pdg.LoadPrev();
        });
        pagingBar.next.click(function () {
            pdg.LoadNext();
        });
        pagingBar.go.click(function () {
            var index = parseInt(pagingBar.index.val());
            if (isNaN(index)) {
                index = 0;
            }
            pagingBar.index.val(index);
            pdg.LoadPage(index);
        });
        pdg.pagingBar = pagingBar;
    }
    pdg.url = e.url;

    pdg.jq_obj.find('thead input[type="checkbox"]').click(function () {
        var checked = this.checked;
        pdg.jq_obj.find('tbody input[type="checkbox"]').each(function () {
            this.checked = checked;
        });
    });

    return pdg;
};

/* iPath标签页切换 */
function iPathTabSheet() { };
iPathTabSheet.fn = iPathTabSheet.prototype;
iPathTabSheet.fn.jqobj = undefined;
iPathTabSheet.fn.Init = function () {
    var that = this;
    that.jqobj.find('li').bind('click', function () {
        that.HideAll();
        var target = $(this).attr('target');
        $('#' + target).show();
        $(this).addClass('checked');
    });
    that.jqobj.find('li:eq(0)').click();
}
iPathTabSheet.fn.HideAll = function () {
    this.jqobj.find('li').each(function () {
        $(this).removeClass('checked');
        var target = $(this).attr('target');
        $('#' + target).hide();
    });
}

jQuery.fn.iPathTabSheet = function () {
    var tabSheet = new iPathTabSheet();
    tabSheet.jqobj = $(this);
    tabSheet.Init();
    return tabSheet;
}


