/*
 *  SHOW UPLOADED IMAGE
 */
function readURL(input, id) {
  let data = new FormData()
  let webFile = ''
  data.append('files', input.files[0])
  // upload image
  $.ajax({
    url: '/api/image/upload/' + id,
    type: 'POST',
    data: data,
    processData: false,
    contentType: false,
    success: function(result) {
      webFile = result.data.succMap['files']
      $('#imageResult').prop('src', webFile)
    },
    error: function() {
      alert('Upload image failed')
    }
  })
}

/*
 *  SHOW UPLOADED IMAGE NAME
 */
let input = document.getElementById( 'upload-image' );
let infoArea = document.getElementById( 'upload-label' );

input.addEventListener( 'change', showFileName );
function showFileName( event ) {
  let input = event.srcElement;
  let fileName = input.files[0].name;
  infoArea.textContent = 'File name: ' + fileName;
}
