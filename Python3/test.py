def isNumber(s):
  try:
    float(s)
    return True
  except ValueError:
    pass
  return False
def redDot(message):
  redFlag = 0
  countChars = 0
  locationOfDot = []
  for c in message:
    if c == "." and redFlag == 1 and countChars == len(message) - 1:
      locationOfDot.append(countChars)
      redFlag = 0
    elif isNumber(c):
      redFlag = 1
    elif c == "." and redFlag == 1:
      redFlag = 2
    elif c == "." and redFlag == 2:
      locationOfDot.append(countChars - 1)
      redFlag = 0
    elif not isNumber(c) and c != "." and redFlag == 2:
      locationOfDot.append(countChars - 1)
      redFlag = 0
    elif not isNumber(c) and c != ".":
      redFlag = 0
    countChars = countChars + 1
  if len(locationOfDot) > 0:
    for i in range(len(locationOfDot) - 1, 0, -1):
      message = message.insert(locationOfDot[i], " ")