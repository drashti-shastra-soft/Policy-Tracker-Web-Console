/* Policy Tracker – shared client helpers (list paging, modals, toasts). */
(function ($) {
    'use strict';

    // ---- $.toast: same call signature AttendEdge views use, rendered with AdminLTE toasts ----
    if (typeof $.toast !== 'function') {
        $.toast = function (opt) {
            opt = opt || {};
            var icon = (opt.icon || 'info').toLowerCase();
            var cls = { success: 'bg-success', error: 'bg-danger', warning: 'bg-warning', info: 'bg-info' }[icon] || 'bg-info';
            if (typeof $(document).Toasts === 'function') {
                $(document).Toasts('create', {
                    class: cls,
                    title: opt.heading || 'Note',
                    body: $('<div/>').text(opt.text || '').html(),
                    autohide: true,
                    delay: opt.hideAfter || 4000,
                    position: 'topRight'
                });
            } else {
                window.alert(opt.text);
            }
        };
    }

    var App = window.App = {};

    App.loading = function (show) { $('#loading-element').toggle(!!show); };

    App.toast = function (type, msg, delay) {
        if (!msg) return;
        $.toast({ text: msg, heading: 'Note', icon: type || 'info', hideAfter: delay || (type === 'error' ? 6000 : 4000) });
    };

    /** Shows {NotificationType, NotificationMsg} returned by JsonNotify. Returns true on success. */
    App.notify = function (data) {
        if (!data) return false;
        if (data.Redirect) { window.location.href = data.Redirect; return false; }
        App.toast(data.NotificationType, data.NotificationMsg);
        return data.NotificationType === 'success';
    };

    App.closeModals = function () {
        $('.modal').modal('hide');
        $('body').removeClass('modal-open');
        $('.modal-backdrop').remove();
    };

    App.refreshList = function () {
        if ($('#frmList').length) { $('#frmList').submit(); } else { window.location.reload(); }
    };

    /** Loads a partial (create / edit / follow-up form) into target and opens the modal inside it. */
    App.openModal = function (url, target) {
        target = target || '#divCreate';
        App.loading(true);
        $.ajax({ url: url, type: 'GET', dataType: 'html', cache: false })
            .done(function (html) {
                $(target).empty().html(html);
                $(target).find('.modal').first().modal('show');
            })
            .always(function () { App.loading(false); });
    };

    /** GET a JsonNotify action (delete, link, rematch…). Refreshes the list on success. */
    App.action = function (url, confirmText, onSuccess) {
        if (confirmText && !window.confirm(confirmText)) return;
        App.loading(true);
        $.ajax({ url: url, type: 'GET', dataType: 'json', cache: false })
            .done(function (data) {
                if (App.notify(data)) {
                    if (typeof onSuccess === 'function') onSuccess(data); else App.refreshList();
                }
            })
            .always(function () { App.loading(false); });
    };

    /** Called at the bottom of _Create/_FollowUp partials with the server notification. */
    App.afterSave = function (type, msg, keepOpen) {
        if (!type || !msg) return;
        App.toast(type, msg);
        if (type === 'success' && !keepOpen) {
            App.closeModals();
            App.refreshList();
        }
    };

    /** Clears the inline error spans and validates required fields marked with data-required. */
    App.validate = function (scope) {
        var ok = true;
        $(scope).find('[data-required]').each(function () {
            var $f = $(this), v = $.trim($f.val() || '');
            var $msg = $f.siblings('span.text-danger').first();
            if (v === '' && $f.is(':visible')) {
                $msg.text(($f.data('required') || 'This field') + ' is required!');
                ok = false;
            } else {
                $msg.text('');
            }
        });
        return ok;
    };

    // ---- list paging (same buttons as AttendEdge) ----
    window.btnPage = function (btn) {
        var $p = $('#pager');
        var currentPage = parseInt($p.data('current')) || 1;
        var totalPage = parseInt($p.data('total')) || 1;
        var pageSize = parseInt($p.data('size')) || 0;
        var $skip = $('#Pagination_Skip');
        var skip = parseInt($skip.val()) || 0;
        if (pageSize <= 0) return;

        switch (btn) {
            case 'First': if (currentPage > 1) { $skip.val(0); $('#frmList').submit(); } break;
            case 'Next': if (currentPage < totalPage) { $skip.val(skip + pageSize); $('#frmList').submit(); } break;
            case 'Last': if (currentPage < totalPage) { $skip.val((totalPage - 1) * pageSize); $('#frmList').submit(); } break;
            case 'Previous': if (skip >= pageSize) { $skip.val(skip - pageSize); $('#frmList').submit(); } break;
        }
    };

    function resetPaging() {
        $('#Pagination_Skip').val(0);
        $('#Pagination_TotalRecord').val(0);
    }

    $(document).on('keyup', '.search-text', function (e) {
        if (e.which === 13) { resetPaging(); $('#frmList').submit(); }
    });
    $(document).on('keydown', '#frmList .search-text', function (e) {
        if (e.which === 13) e.preventDefault();
    });
    $(document).on('change', '.search-dropdown', function () { resetPaging(); $('#frmList').submit(); });
    $(document).on('change', '.filter-pagination', function () { resetPaging(); $('#frmList').submit(); });

    // ---- session expiry on ajax calls ----
    $(document).ajaxError(function (e, xhr) {
        if (xhr.status === 401) {
            var r = xhr.responseJSON || {};
            App.toast('error', r.NotificationMsg || 'Your session has expired. Please sign in again.');
            setTimeout(function () { window.location.href = r.Redirect || '/Authentication'; }, 1200);
        } else if (xhr.status >= 500) {
            App.toast('error', 'Internal Server Error!');
        }
    });

    // Ajax.BeginForm toggles #loading-element itself; make sure it is hidden after failures too.
    $(document).ajaxStop(function () { App.loading(false); });
})(jQuery);
