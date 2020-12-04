
function ChooseUser() { }
ChooseUser.fn = ChooseUser.prototype;
ChooseUser.fn.$deptTree = undefined;
ChooseUser.fn.$deptTree_users = undefined;
ChooseUser.fn.$chooseduser = undefined;

ChooseUser.fn.Init = function () {
    var that = this;
    $('#deptRoot').click(function () {
        $(this).parent().nextAll().remove();
        that.ForwardDept('00000000-0000-0000-0000-000000000000');
        $('#keyword').val('');
    });
    $('#btnCheckedAllUser').click(function () {
        that.CheckedAllUser();
    });
    $('#keyword').bind('input propertychange', function () {
        that.Search();
    });

    that.$deptTree = $('#deptTree').iPathDataBind();
    that.$deptTree_users = $('#deptTree_users').iPathDataBind();
    that.$chooseduser = $('#chooseduser').iPathDataBind();

    iPath.Post(contextUri + '/Admin/QyUser/LoadALL', { },
            function (d) {
                if (d.state == 1) {
                    that.data = d.data;

                    for (var i in that.data.dept_users) {
                        var dept_user = that.data.dept_users[i];
                        var user = iPath.Where(that.data.users, function (d) {
                            if (d.ID == dept_user.QyUserID) {
                                return d
                            }
                            else {
                                return undefined;
                            }
                        });
                        dept_user.user = user[0];
                    }
                    $('#deptRoot').click();

                } else {
                    $showdialog({ title: '出错了', body: d.txt });
                }
            }, 'json');
}

// 导航到某一层级
ChooseUser.fn.ForwardDept = function (ParentID, Drawpathlink) {
    var that = this;

    if (Drawpathlink) {
        var pathlink = $("<div class='item'> &nbsp; > <a>全部</a></div>");
        var dept = iPath.Where(that.data.depts, function (d) {
            if (d.ID == ParentID)
                return d;
            else
                return undefined;
        });

        pathlink.find('a').text(dept[0].Name);
        pathlink.find('a').attr('_id', ParentID);
        pathlink.find('a').click(function () {
            var _id = $(this).attr('_id');
            that.ForwardDept(_id, true);
            $(this).parent().nextAll().remove();
        });

        $('.pathlink').append(pathlink);
    }

    // 找到子组织结构
    var listDept = iPath.Where(that.data.depts, function (d) {
        if (d.ParentID == ParentID)
            return d;
        else
            return undefined;
    });
    that.$deptTree.DataBind(listDept);
    $('#deptTree>li>label').click(function () {
        that.ForwardDept($(this).attr('_id'), true);
    });

    // 找到成员
    var listUser = iPath.Where(that.data.dept_users, function (d) {
        if (d.QyDeptID == ParentID)
            return d.user;
        else
            return undefined;
    });
    that.ShowUser(listUser);
}

// 显示组织下属员工
ChooseUser.fn.ShowUser = function (listUser) {
    var that = this;
    that.$deptTree_users.DataBind(listUser);
    $('#deptTree_users input[type="checkbox"]').click(function () {
        var id = $(this).attr('_id');
        var name = $(this).attr('_name');
        var checked = $(this)[0].checked;
        if (checked) {
            // 选中
            that.CheckedUser(id, name);
        } else {
            // 取消
            $('.choosed .item[_id="' + id + '"]').remove();
        }
    });
    that.RefreshLiChecked();
    if (listUser.length > 0) {
        $('#footbar').show();
    } else {
        $('#footbar').hide();
    }
}

ChooseUser.fn.UnCheckedUser = function (id) {
    $('.choosed .item[_id="' + id + '"]').remove();
    this.RefreshLiChecked();
}

// 点选组织列表中已经被选中的成员
ChooseUser.fn.RefreshLiChecked = function () {
    $('#deptTree_users input[type="checkbox"]').each(function () {
        var id = $(this).attr('_id');
        $(this)[0].checked = $('#chooseduser .item[_id="' + id + '"]').length > 0;
    });
}

// 选中单个员工
ChooseUser.fn.CheckedUser = function (id, name) {
    var that = this;
    that.$chooseduser.AppendDataBind([{ id: id, name: name }]);
    $('#chooseduser .item:last .remove').click(function () {
        that.UnCheckedUser($(this).parent().attr('_id'));
    });
}

// 权重当前所有员工
ChooseUser.fn.CheckedAllUser = function () {
    var that = this;
    $('#deptTree_users input[type="checkbox"]').each(function () {
        var id = $(this).attr('_id');
        var name = $(this).attr('_name');
        if ($(this)[0].checked == false) {
            $(this)[0].checked = true;
            that.CheckedUser(id, name);
        }
    });
}

// 搜索
ChooseUser.fn.Search = function () {
    var that = this;
    var key = $('#keyword').val();
    if (key == '') {
        $('#deptRoot').click();
        return;
    }
    that.$deptTree.DataBind([]);
    // 找到成员
    var listUser = iPath.Where(that.data.users, function (d) {
        if (d.Name.indexOf(key) > -1
            || d.Email.indexOf(key) > -1
            || d.UserId.indexOf(key) > -1)
            return d;
        else
            return undefined;
    });
    that.ShowUser(listUser);
}

// 获取选中的员工
ChooseUser.fn.GetChecked = function () {
    var ids = new Array();
    $('#chooseduser .item').each(function () {
        var id = $(this).attr('_id');
        ids.push(id);
    });
    return ids;
}

