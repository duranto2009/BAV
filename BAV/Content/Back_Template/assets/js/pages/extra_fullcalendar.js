/* ------------------------------------------------------------------------------
*
*  # Fullcalendar basic options
*
*  Specific JS code additions for extra_fullcalendar_views.html and 
*  extra_fullcalendar_styling.html pages
*
*  Version: 1.0
*  Latest update: Aug 1, 2015
*
* ---------------------------------------------------------------------------- */

$(function() {


    // Add events
    // ------------------------------

    // Default events
    var events = [
        {
            title: 'All Day Event',
            start: '2014-11-01'
        },
        {
            title: 'Long Event',
            start: '2014-11-07',
            end: '2014-11-10'
        },
        {
            id: 999,
            title: 'Repeating Event',
            start: '2014-11-09T16:00:00'
        },
        {
            id: 999,
            title: 'Repeating Event',
            start: '2014-11-16T16:00:00'
        },
        {
            title: 'Conference',
            start: '2014-11-11',
            end: '2014-11-13'
        },
        {
            title: 'Meeting',
            start: '2014-11-12T10:30:00',
            end: '2014-11-12T12:30:00'
        },
        {
            title: 'Lunch',
            start: '2014-11-12T12:00:00'
        },
        {
            title: 'Meeting',
            start: '2014-11-12T14:30:00'
        },
        {
            title: 'Happy Hour',
            start: '2014-11-12T17:30:00'
        },
        {
            title: 'Dinner',
            start: '2014-11-12T20:00:00'
        },
        {
            title: 'Birthday Party',
            start: '2014-11-13T07:00:00'
        },
        {
            title: 'Click for Google',
            url: 'http://google.com/',
            start: '2014-11-28'
        }
    ];


    // Event colors
    var eventColors = [
        {
            title: 'All Day Event',
            start: '2014-11-01',
            color: '#EF5350'
        },
        {
            title: 'Long Event',
            start: '2014-11-07',
            end: '2014-11-10',
            color: '#26A69A'
        },
        {
            id: 999,
            title: 'Repeating Event',
            start: '2014-11-09T16:00:00',
            color: '#26A69A'
        },
        {
            id: 999,
            title: 'Repeating Event',
            start: '2014-11-16T16:00:00',
            color: '#5C6BC0'
        },
        {
            title: 'Conference',
            start: '2014-11-11',
            end: '2014-11-13',
            color: '#546E7A'
        },
        {
            title: 'Meeting',
            start: '2014-11-12T10:30:00',
            end: '2014-11-12T12:30:00',
            color: '#546E7A'
        },
        {
            title: 'Lunch',
            start: '2014-11-12T12:00:00',
            color: '#546E7A'
        },
        {
            title: 'Meeting',
            start: '2014-11-12T14:30:00',
            color: '#546E7A'
        },
        {
            title: 'Happy Hour',
            start: '2014-11-12T17:30:00',
            color: '#546E7A'
        },
        {
            title: 'Dinner',
            start: '2014-11-12T20:00:00',
            color: '#546E7A'
        },
        {
            title: 'Birthday Party',
            start: '2014-11-13T07:00:00',
            color: '#546E7A'
        },
        {
            title: 'Click for Google',
            url: 'http://google.com/',
            start: '2014-11-28',
            color: '#FF7043'
        }
    ];


    // Event background colors
    var eventBackgroundColors = [
        {
            title: 'All Day Event',
            start: '2014-11-01'
        },
        {
            title: 'Long Event',
            start: '2014-11-07',
            end: '2014-11-10',
            color: '#DCEDC8',
            rendering: 'background'
        },
        {
            id: 999,
            title: 'Repeating Event',
            start: '2014-11-06T16:00:00'
        },
        {
            id: 999,
            title: 'Repeating Event',
            start: '2014-11-16T16:00:00'
        },
        {
            title: 'Conference',
            start: '2014-11-11',
            end: '2014-11-13'
        },
        {
            title: 'Meeting',
            start: '2014-11-12T10:30:00',
            end: '2014-11-12T12:30:00'
        },
        {
            title: 'Lunch',
            start: '2014-11-12T12:00:00'
        },
        {
            title: 'Happy Hour',
            start: '2014-11-12T17:30:00'
        },
        {
            title: 'Dinner',
            start: '2014-11-24T20:00:00'
        },
        {
            title: 'Meeting',
            start: '2014-11-03T10:00:00'
        },
        {
            title: 'Birthday Party',
            start: '2014-11-13T07:00:00'
        },
        {
            title: 'Vacation',
            start: '2014-11-27',
            end: '2014-11-30',
            color: '#FFCCBC',
            rendering: 'background'
        }
    ];



    // Initialization
    // ------------------------------

    // Basic view
    $('.fullcalendar-basic').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,basicWeek,basicDay'
        },
        defaultDate: new Date(),
        editable: true,
        events: function (start, end, timezone, callback) {
            $.ajax({
                url : '/Home/getEvents',
                dataType: "json",
                data: {
                    start: start.unix(),
                    end: end.unix()
                },
                success: function (doc) {
                    var events = [];
                    $.each(doc, function (i, item) {
                        //alert(item["title"]);
                        events.push({
                            id: $(this).attr('Id'),
                            title: $(this).attr('EventName'),
                            start: $(this).attr('Date'), // will be parsed
                            end: $(this).attr('enddate'),
                            Area: $(this).attr('Area')
                        });
                    });
                    callback(events);
                }
            });
        },
        //eventClick: function (calEvent, jsEvent, view) {
        //    alert('You clicked on event id: ' + calEvent.id
        //        + "\nAnd the title is: " + calEvent.title);

        //},
        eventClick: function (event, jsEvent, view) {
            $('#modalTitle').html(event.title);
            $('#modalBody').html("<b>এলাকাঃ</b> " + event.Area);
            $('#eventUrl').attr('href', event.url);
            $('#fullCalModal').modal();
        }
    });
  
    //events: function (start, end, timezone, callback) {
    //    $.ajax({
    //        url: '/Home/getEvents',
    //        type: "GET",
    //        datatype: 'json',
    //        success: function (doc) {
    //            console.log(doc);
    //            var eventsdata = [];
    //            if (doc != undefined && doc.length > 0) {
    //                doc.forEach(function (entry) {
    //                    eventsdata.push({
    //                        title: entry.EventName,
    //                        start: entry.Date,
    //                        end: entry.enddate
    //                    });
    //                });
    //            }
                    
    //            callback(eventsdata);
    //        }, error: function (err) {
    //            alert('Error in fetching data');
    //        }
    //    });
    //},

   

    // Agenda view
    $('.fullcalendar-agenda').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        defaultDate: '2014-11-12',
        defaultView: 'agendaWeek',
        editable: true,
        events: events
    });


    // Event colors
    $('.fullcalendar-event-colors').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        defaultDate: '2014-11-12',
        editable: true,
        events: eventColors
    });


    // Event background colors
    $('.fullcalendar-background-colors').fullCalendar({
        header: {
            left: 'prev,next today',
            center: 'title',
            right: 'month,agendaWeek,agendaDay'
        },
        defaultDate: '2014-11-12',
        editable: true,
        events: eventBackgroundColors
    });

});
