/// <reference name="MicrosoftAjax.js" />
/// <reference name="dnn.js" assembly="DotNetNuke.WebUtility" />
/// <reference name="dnn.xmlhttp.js" assembly="DotNetNuke.WebUtility" />

Type.registerNamespace('WinDoH');

WinDoH.ViewKeywordification = function() {
    //Call Base Method
    WinDoH.ViewKeywordification.initializeBase(this);
    //Member Variables
    this._msgs = {};
    this._loadDataButton = null;
    this._cancelButton = null;
    this._lasttabid = null;
    this._portalId = null;

    //Associated delegates to single member variable dictionary to make it easy to dispose
    this._delegates = {
        _loadDataSuccessDelegate: Function.createDelegate(this, this._loadDataSuccess),
        _loadDataFailDelegate: Function.createDelegate(this, this._loadDataFail),
        _onLoadDelegate: Function.createDelegate(this, this._onLoad),

        _onGridSaveDelegate: Function.createDelegate(this, this._onGridSave),
        _onGridSavedDelegate: Function.createDelegate(this, this._onGridSaved),

        componentPropChangedDelegate: Function.createDelegate(this, this._onPropChanged)
    };

    //Event Hookup
    Sys.Application.add_load(this._delegates._onLoadDelegate);
}

WinDoH.ViewKeywordification.prototype =
{
    //Properties
    get_ns: function() { return this.get_id() + '_'; },
    get_msgs: function() { return this._msgs; },
    set_msgs: function(value) { this._msgs = Sys.Serialization.JavaScriptSerializer.deserialize(value); },
    get_lasttabid: function() { return this._lasttabid; },
    set_lasttabid: function(value) { this._lasttabid = value; },
    get_PortalId: function() { return this._portalId; },
    set_PortalId: function(value) { this._portalId = value; },
    
    //Events
    initialize: function() {
        //Call Base Method    
        WinDoH.ViewKeywordification.callBaseMethod(this, 'initialize');

        //create UI
        this._loadDataButton = this._createChildControl('btnLoadData', 'input', 'button');
        this._loadDataButton.value = this.getMessage('bFetchData.Initial'); //get localized value
        $get(this.get_ns() + 'pnl').parentNode.appendChild(this._loadDataButton);

        this._cancelButton = this._createChildControl('btnCancelEditRow', 'input', 'button');
        this._cancelButton.value = this.getMessage('bFetchData.Cancel'); //get localized value
        this._cancelButton.style.display = 'none';
        $get(this.get_ns() + 'pnl').parentNode.appendChild(this._cancelButton);

        //hookup event handlers
        $addHandlers(this._loadDataButton, { "click": this._onLoadData }, this);
        $addHandlers(this._cancelButton, { "click": this._onGridRowCancel }, this);
    },

    _onLoad: function(src, arg) {
        //page is completely loaded, you can now access any element or component
        var components = arg.get_components();
        //sample of how to do client-side IMC - look for other components we are interested in
        //in this case, we will look for other instances of the same module that are not ourselves
        //but you can communicate with any components
        for (var i = 0; i < components.length; i++) {
            if (Object.getTypeName(components[i]) == Object.getTypeName(this) && components[i].get_id() != this.get_id())
                this.add_propertyChanged(components[i]._delegates.componentPropChangedDelegate);
        }
    },

    _onLoadData: function(src, arg) {
        this._displayWait(true);

        $('#' + this.get_ns() + 'jqgrid').jqGrid({
            url: null,
            editurl: 'clientArray',
            datatype: 'local',
            colNames: ['#',this.getMessage('Column.Priority'), this.getMessage('Column.TabName'), this.getMessage('Column.Title'), this.getMessage('Column.Keywords'), this.getMessage('Column.Description')],
            colModel: [
                { name: 'TabID', index: 'TabID', hidden: true },
                { name: 'SiteMapPriority', index: 'SiteMapPriority',width:30, hidden:false, editable:true, align: 'center', resizable:false },
                { name: 'TabName', index: 'TabName', width:100, hidden: false, editable: true },
                { name: 'Title', index: 'Title', width: 175, editable: true, edittype: 'textarea', editoptions:{maxlength:'69', rows: 3} },
                { name: 'KeyWords', index: 'KeyWords', width: 100, editable: true, edittype: 'textarea', editoptions: { rows: 3}},
                { name: 'Description', index: 'Description', width: 400, editable: true, edittype: 'textarea', editoptions: {maxlength:'170',  rows: 3} }
            ],
            jsonReader: {
                root: "rows",
                id: "TabID",
                index: "TabID",
                page: "page",
                total: "total",
                records: "records",
                repeatitems: false
            },
            sortname: 'TabID',
            viewrecords: true,
            rowNum:-1, 
            sortorder: "desc",
            height: "auto",
            width: "960",
            caption: this.getMessage('Grid.Title'),
            onSelectRow: function(ids) {
                var pd = $('#' + this.id).getPostData();
                var r = null;
                $.each(pd, function(i) { if (i == 'baseclass') { r = pd[i]; } });


                if (ids && ids !== r.get_lasttabid) {
                    $('#' + this.id).editRow(ids, false, r._delegates._onGridSaveDelegate);
                    r.set_lasttabid(ids);
                }
            }
        });
       
        var tempthis = this;
        $('#' + this.get_ns() + 'jqgrid').setPostData({ baseclass: tempthis });



        dnn.xmlhttp.callControlMethod('WinDoH.ViewKeywordification.' + this.get_id(),
            'GetPortalTabs', { portalId: this.get_PortalId() }, this._delegates._loadDataSuccessDelegate, this._delegates._loadDataFailDelegate);

        this.raisePropertyChanged('LoadedData');
    },

    _onPropChanged: function(src, args) {
        this.showMessage(String.format('You {0} to {1} but not to me?', args.get_propertyName(), src.get_name()));
    },

    _onGridSaved: function(src, arg) {
        this.raisePropertyChanged('RowUpdated');
    },
    _onGridSave: function(src, arg) {

        if (this.get_lasttabid()) {

            $('#' + this.get_ns() + 'jqgrid').saveRow(this.get_lasttabid());

            var ret = $('#' + this.get_ns() + 'jqgrid').getRowData(this.get_lasttabid());

            dnn.xmlhttp.callControlMethod('WinDoH.ViewKeywordification.' + this.get_id(), 'SaveGridRow', { tabId: ret.TabID, name: ret.TabName, title: ret.Title, description: ret.Description, keywords: ret.KeyWords, priority: ret.SiteMapPriority }, this._delegates._onGridSavedDelegate, this._delegates._loadDataFailDelegate);

        }
    },
    _onGridRowCancel: function(src, arg) {
        if (this.get_lasttabid()) {
            $('#' + this.get_ns() + 'jqgrid').restoreRow(this.get_lasttabid());
        }
    },

    //Methods
    getMessage: function(key) {
        return this._msgs[key];
    },

    showMessage: function(msg) {
        $get(this.get_ns() + 'lblResponse').innerHTML = msg;
    },

    //Private Methods
    _createChildControl: function(id, tag, type) {
        var ctl = document.createElement(tag);
        ctl.id = this.get_ns() + id;
        if (type)
            ctl.type = type;
        return ctl;
    },

    _displayWait: function(show) {
        $get(this.get_ns() + 'imgAjax').className = (show ? '' : 'ceHidden');
    },

    _loadDataSuccess: function (payload, ctx, req) {
        this._displayWait(false);

        $('#'+ this.get_ns() + 'btnCancelEditRow').show();
        this._loadDataButton.value = this.getMessage('bFetchData.Refresh');

        var myjqg = $('#' + this.get_ns() + 'jqgrid')[0];
        myjqg.addJSONData(payload);
    },

    _loadDataFail: function (payload, ctx, req) {
        this._displayWait(false);
        alert('error: ' + payload);
    },

    dispose: function() {
        $clearHandlers(this._loadDataButton);
        this._loadDataButton = null;
        this._delegates = null;
        WinDoH.ViewKeywordification.callBaseMethod(this, 'dispose');
    }
}

//register class and inherit from Sys.Component
WinDoH.ViewKeywordification.registerClass('WinDoH.ViewKeywordification', Sys.Component);
