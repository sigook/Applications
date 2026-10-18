Uri mapsSearchUri(double latitude, double longitude) {
  return Uri.https('www.google.com', '/maps/search/', {
    'api': '1',
    'query': '$latitude,$longitude',
  });
}

Uri? staticMapUri(
  double latitude,
  double longitude, {
  required String apiKey,
  int width = 640,
  int height = 320,
  int zoom = 15,
}) {
  if (apiKey.isEmpty) return null;
  final point = '$latitude,$longitude';
  return Uri.https('maps.googleapis.com', '/maps/api/staticmap', {
    'center': point,
    'zoom': '$zoom',
    'size': '${width}x$height',
    'scale': '2',
    'markers': 'color:red|$point',
    'key': apiKey,
  });
}
