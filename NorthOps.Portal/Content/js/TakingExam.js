var ChoiceId = '';
AddAntiForgeryToken = function (data, form) {
    data.__RequestVerificationToken = $('#' + form + ' input[name=__RequestVerificationToken]').val();
    return data;
};
$(document).ready(function () {

    audio = document.getElementsByTagName('audio');
});
function performAjax(ctrl) {

    $.ajax({
        url: ctrl.attr('action'),
        data: AddAntiForgeryToken({ ChoiceId: ChoiceId }, $(ctrl).attr('id')),
        type: 'POST',
        beforeSend: function () {
            loadingPanel.Show();
        },
        complete: function () {
            loadingPanel.Hide();
            var indexTab = (QuestionPageControl.GetActiveTabIndex());
            indexTab++;
            QuestionPageControl.SetActiveTabIndex(indexTab);
            if (indexTab >= tabs) {
                stopTimer();
            }

        }
    });
}
function performIdentificationAjax(ctrl) {
    $.ajax({
        url: ctrl.attr('action'),
        type: 'POST',
        data: {
            choice: choice.GetValue(),
            sessionId: $('#SessionId').val(),
            questionId: $('#QuestionId').val(),
            item: $('#Item').val()
        },

        beforeSend: function () {
            LoadingOpenExam.Show();
        },
        success: function(e) {
            $('#frm_identification_exam').html(e);
        },
        complete: function (data) {
            LoadingOpenExam.Hide();
           
           
        }
    });
}
function NextTab() {
    var indexTab = (QuestionPageControl.GetActiveTabIndex());
    indexTab++;
    QuestionPageControl.SetActiveTabIndex(indexTab);

    if (timerStart === false) {
        timer = setInterval(countDown, 1000);
        timerStart = true;

    }
    if (indexTab >= tabs) {
        stopTimer();
    }
    audio.pause();
}
var min = parseInt(0);
var sec = parseInt(0);
var hr = parseInt(0);
var timer;
var timerStart = false;

function countDown() {
    sec++;
    if (sec === 60) {
        sec = 0;
        min++;
    }
    if (min === 60) {
        hr++;
        min = 0;
    }
    var time = toFixedLength(hr, 2) + ":" + toFixedLength(min, 2) + ":" + toFixedLength(sec, 2);
    $(lbltimer).text(time);
    if (min > duration) {
        stopTimer();
        QuestionPageControl.SetActiveTabIndex(QuestionPageControl.GetTabCount() - 1);
    }
}
function startTimer(s, e) {
    if (timerStart === false) {
        timer = setInterval(countDown, 1000);
        timerStart = true;

    }
}
function OnPlayVideo(ctrl) {
    if (timerStart === false) {
        timer = setInterval(countDown, 1000);
        timerStart = true;
    }
    $(ctrl).attr('controls', '');
}
function stopTimer() {
    clearInterval(timer);
}
function toFixedLength(input, length, padding) {
    padding = String(padding || "0");
    return (padding.repeat(length) + input).slice(-length);
}
$('#videoExam').mousedown(function (event) {
    switch (event.which) {
        case 1:

            break;
        case 2:
            break;
        case 3:
            alert('Invalid Response');
            break;
        default:
            break;
    }
});
var audio;
var _audioLength;
var _audioTimer;
var _audioCountDown = 0;
var _btnNextSrc;
var _ctrl;
function playAudio(ctrl, url, src) {
    audio = document.getElementById('#audio-' + src);
    _ctrl = ctrl;
    LoadingOpenExam.Show();
    _btnNextSrc = src;

    //  console.log(replaceAll(src, '-', ''));

    getDuration(url).then(function (audio1) {
        LoadingOpenExam.Hide();
        audio1.play();
        $(_ctrl).html("<span class='fa fa-pause'></span>");
        _audioLength = audio1.duration;
        console.log(_audioLength);
        _audioTimer = setInterval(audioCountDown, 1000);
        $('#h_' + _btnNextSrc).text('Playing Audio');
        $(ctrl).attr('onclick', '');
    });

    //_audioLength = audio.duration;
    //console.log(_audioLength);
    //_audioTimer = setInterval(audioCountDown, 1000);
}
function audioCountDown() {

    if (_audioCountDown > _audioLength) {
        $(_ctrl).html("<span class='fa fa-stop'></span>");
        eval('btnNext' + replaceAll(_btnNextSrc, '-', '')).SetEnabled(true);
        clearInterval(_audioTimer);
        _audio.pause();
        $('#h_' + _btnNextSrc).text('Audio Stop');
    }
    console.log(_audioCountDown);
    _audioCountDown++;
}

var _audio;
function getDuration(src) {
    return new window.Promise(function (resolve) {
        _audio = new Audio();
        $(_audio).on("loadedmetadata", function () {
            resolve(_audio);
        });
        _audio.src = src;
    });
}
function escapeRegExp(str) {
    return str.replace(/([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1");
}
function replaceAll(str, find, replace) {
    return str.replace(new RegExp(escapeRegExp(find), 'g'), replace);
}