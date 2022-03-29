// The purpose of this program is to print the offsets of the fields of the various structs used by libvncserver.
// This data can then feed into the P/Invoke code, to ensure the correct offsets are used.
#include <stddef.h>
#include <stdio.h>
#include <rfb/rfb.h>

int main()
{
	printf("Defines\n");
	printf("-------\n");

#ifdef LIBVNCSERVER_HAVE_LIBZ
	printf("LIBVNCSERVER_HAVE_LIBZ\n");
#endif
#ifdef LIBVNCSERVER_HAVE_LIBJPEG
	printf("LIBVNCSERVER_HAVE_LIBJPEG\n");
#endif
#ifdef LIBVNCSERVER_HAVE_LIBPNG
	printf("LIBVNCSERVER_HAVE_LIBPNG\n");
#endif
#ifdef LIBVNCSERVER_HAVE_LIBPTHREAD
	printf("LIBVNCSERVER_HAVE_LIBPTHREAD\n");
#endif
#ifdef LIBVNCSERVER_HAVE_WIN32THREADS
	printf("LIBVNCSERVER_HAVE_WIN32THREADS\n");
#endif
#ifdef TODELETE
	printf("TODELETE");
#endif

	printf("\n");
	printf("Sizes\n");
	printf("-----\n");

	printf("sizeof(void*): %zu\n", sizeof(void*));
	printf("sizeof(int): %zu\n", sizeof(int));
	printf("sizeof(rfbPixel ): %zu\n", sizeof(rfbPixel));
	printf("sizeof(rfbPixelFormat): %zu\n", sizeof(rfbPixelFormat));
	printf("sizeof(rfbColourMap): %zu\n", sizeof(rfbColourMap));
#ifdef __linux__
	printf("sizeof(SOCKET): %zu\n", sizeof(SOCKET));
#else
	printf("sizeof(rfbSocket): %zu\n", sizeof(rfbSocket));
#endif
#ifdef WIN32
	printf("sizeof(struct fd_set): %zu\n", sizeof(struct fd_set));
#else
	printf("sizeof(fd_set): %zu\n", sizeof(fd_set));
#endif
	printf("sizeof(enum rfbSocketState): %zu\n", sizeof(enum rfbSocketState));
	printf("sizeof(struct sockaddr_in): %zu\n", sizeof(struct sockaddr_in));
	printf("sizeof(rfbColourMap ): %zu\n", sizeof(rfbColourMap));
	printf("sizeof(in_addr_t): %zu\n", sizeof(in_addr_t));
	printf("sizeof(float): %zu\n", sizeof(float));
	printf("sizeof(struct timeval): %zu\n", sizeof(struct timeval));

#ifdef LIBVNCSERVER_HAVE_LIBZ
	printf("sizeof(struct z_stream_s): %zu\n", sizeof(struct z_stream_s));
#endif

#ifdef LIBVNCSERVER_HAVE_LIBPTHREAD
	printf("sizeof(pthread_mutex_t): %zu\n", sizeof(pthread_mutex_t));
#endif

#ifdef LIBVNCSERVER_HAVE_WIN32THREADS
   printf("sizeof(CRITICAL_SECTION): %zu\n", sizeof(CRITICAL_SECTION));
#endif

#ifdef LIBVNCSERVER_HAVE_LIBJPEG
	printf("sizeof(z_stream): %zu\n", sizeof(z_stream));
#endif

	printf("sizeof(rfbFileTransferData): %zu\n", sizeof(rfbFileTransferData));
	
#ifdef LIBVNCSERVER_HAVE_LIBPTHREAD
	printf("sizeof(pthread_cond_t): %zu\n", sizeof(pthread_cond_t));
	printf("sizeof(pthread_t): %zu\n", sizeof(pthread_t));
#endif

	printf("sizeof(rfbScreenInfo): %zu\n", sizeof(rfbScreenInfo));
	
	printf("\n");
	printf("Offsets\n");
	printf("-------\n");

	printf("offsetof(rfbScreenInfo, scaledScreenNext) : %zu\n", offsetof(rfbScreenInfo, scaledScreenNext));
	printf("offsetof(rfbScreenInfo, scaledScreenRefCount) : %zu\n", offsetof(rfbScreenInfo, scaledScreenRefCount));

	printf("offsetof(rfbScreenInfo, width) : %zu\n", offsetof(rfbScreenInfo, width));
	printf("offsetof(rfbScreenInfo, paddedWidthInBytes) : %zu\n", offsetof(rfbScreenInfo, paddedWidthInBytes));
	printf("offsetof(rfbScreenInfo, height) : %zu\n", offsetof(rfbScreenInfo, height));
	printf("offsetof(rfbScreenInfo, depth) : %zu\n", offsetof(rfbScreenInfo, depth));
	printf("offsetof(rfbScreenInfo, bitsPerPixel) : %zu\n", offsetof(rfbScreenInfo, bitsPerPixel));
	printf("offsetof(rfbScreenInfo, sizeInBytes) : %zu\n", offsetof(rfbScreenInfo, sizeInBytes));

	printf("offsetof(rfbScreenInfo, blackPixel) : %zu\n", offsetof(rfbScreenInfo, blackPixel));
	printf("offsetof(rfbScreenInfo, whitePixel) : %zu\n", offsetof(rfbScreenInfo, whitePixel));

	printf("offsetof(rfbScreenInfo, screenData) : %zu\n", offsetof(rfbScreenInfo, screenData));

	printf("offsetof(rfbScreenInfo, serverFormat) : %zu\n", offsetof(rfbScreenInfo, serverFormat));
	printf("offsetof(rfbScreenInfo, colourMap) : %zu\n", offsetof(rfbScreenInfo, colourMap));
	printf("offsetof(rfbScreenInfo, desktopName) : %zu\n", offsetof(rfbScreenInfo, desktopName));
	printf("offsetof(rfbScreenInfo, thisHost) : %zu\n", offsetof(rfbScreenInfo, thisHost));

	printf("offsetof(rfbScreenInfo, autoPort) : %zu\n", offsetof(rfbScreenInfo, autoPort));
	printf("offsetof(rfbScreenInfo, port) : %zu\n", offsetof(rfbScreenInfo, port));
	printf("offsetof(rfbScreenInfo, listenSock) : %zu\n", offsetof(rfbScreenInfo, listenSock));
	printf("offsetof(rfbScreenInfo, maxSock) : %zu\n", offsetof(rfbScreenInfo, maxSock));
	printf("offsetof(rfbScreenInfo, maxFd) : %zu\n", offsetof(rfbScreenInfo, maxFd));
	printf("offsetof(rfbScreenInfo, allFds) : %zu\n", offsetof(rfbScreenInfo, allFds));

	printf("offsetof(rfbScreenInfo, socketState) : %zu\n", offsetof(rfbScreenInfo, socketState));
	printf("offsetof(rfbScreenInfo, inetdSock) : %zu\n", offsetof(rfbScreenInfo, inetdSock));
	printf("offsetof(rfbScreenInfo, inetdInitDone) : %zu\n", offsetof(rfbScreenInfo, inetdInitDone));

	printf("offsetof(rfbScreenInfo, udpPort) : %zu\n", offsetof(rfbScreenInfo, udpPort));
	printf("offsetof(rfbScreenInfo, udpSock) : %zu\n", offsetof(rfbScreenInfo, udpSock));
	printf("offsetof(rfbScreenInfo, udpClient) : %zu\n", offsetof(rfbScreenInfo, udpClient));
	printf("offsetof(rfbScreenInfo, udpSockConnected) : %zu\n", offsetof(rfbScreenInfo, udpSockConnected));
	printf("offsetof(rfbScreenInfo, udpRemoteAddr) : %zu\n", offsetof(rfbScreenInfo, udpRemoteAddr));

	printf("offsetof(rfbScreenInfo, maxClientWait) : %zu\n", offsetof(rfbScreenInfo, maxClientWait));

	/* http stuff */
	printf("offsetof(rfbScreenInfo, httpInitDone) : %zu\n", offsetof(rfbScreenInfo, httpInitDone));
	printf("offsetof(rfbScreenInfo, httpEnableProxyConnect) : %zu\n", offsetof(rfbScreenInfo, httpEnableProxyConnect));
	printf("offsetof(rfbScreenInfo, httpPort) : %zu\n", offsetof(rfbScreenInfo, httpPort));
	printf("offsetof(rfbScreenInfo, httpDir) : %zu\n", offsetof(rfbScreenInfo, httpDir));
	printf("offsetof(rfbScreenInfo, httpListenSock) : %zu\n", offsetof(rfbScreenInfo, httpListenSock));
	printf("offsetof(rfbScreenInfo, httpSock) : %zu\n", offsetof(rfbScreenInfo, httpSock));

	printf("offsetof(rfbScreenInfo, passwordCheck) : %zu\n", offsetof(rfbScreenInfo, passwordCheck));
	printf("offsetof(rfbScreenInfo, authPasswdData) : %zu\n", offsetof(rfbScreenInfo, authPasswdData));
	printf("offsetof(rfbScreenInfo, authPasswdFirstViewOnly) : %zu\n", offsetof(rfbScreenInfo, authPasswdFirstViewOnly));

	printf("offsetof(rfbScreenInfo, deferUpdateTime) : %zu\n", offsetof(rfbScreenInfo, deferUpdateTime));
#ifdef TODELETE
	printf("offsetof(rfbScreenInfo, screen) : %zu\n", offsetof(rfbScreenInfo, screen));
#endif
	printf("offsetof(rfbScreenInfo, alwaysShared) : %zu\n", offsetof(rfbScreenInfo, alwaysShared));
	printf("offsetof(rfbScreenInfo, neverShared) : %zu\n", offsetof(rfbScreenInfo, neverShared));
	printf("offsetof(rfbScreenInfo, dontDisconnect) : %zu\n", offsetof(rfbScreenInfo, dontDisconnect));
	printf("offsetof(rfbScreenInfo, clientHead) : %zu\n", offsetof(rfbScreenInfo, clientHead));
	printf("offsetof(rfbScreenInfo, pointerClient) : %zu\n", offsetof(rfbScreenInfo, pointerClient));

	printf("offsetof(rfbScreenInfo, cursorX) : %zu\n", offsetof(rfbScreenInfo, cursorX));
	printf("offsetof(rfbScreenInfo, cursorY) : %zu\n", offsetof(rfbScreenInfo, cursorY));
	printf("offsetof(rfbScreenInfo, underCursorBufferLen) : %zu\n", offsetof(rfbScreenInfo, underCursorBufferLen));
	printf("offsetof(rfbScreenInfo, underCursorBuffer) : %zu\n", offsetof(rfbScreenInfo, underCursorBuffer));
	printf("offsetof(rfbScreenInfo, dontConvertRichCursorToXCursor) : %zu\n", offsetof(rfbScreenInfo, dontConvertRichCursorToXCursor));
	printf("offsetof(rfbScreenInfo, cursor) : %zu\n", offsetof(rfbScreenInfo, cursor));

	printf("offsetof(rfbScreenInfo, frameBuffer) : %zu\n", offsetof(rfbScreenInfo, frameBuffer));
	printf("offsetof(rfbScreenInfo, kbdAddEvent) : %zu\n", offsetof(rfbScreenInfo, kbdAddEvent));
	printf("offsetof(rfbScreenInfo, kbdReleaseAllKeys) : %zu\n", offsetof(rfbScreenInfo, kbdReleaseAllKeys));
	printf("offsetof(rfbScreenInfo, ptrAddEvent) : %zu\n", offsetof(rfbScreenInfo, ptrAddEvent));
	printf("offsetof(rfbScreenInfo, setXCutText) : %zu\n", offsetof(rfbScreenInfo, setXCutText));
	printf("offsetof(rfbScreenInfo, getCursorPtr) : %zu\n", offsetof(rfbScreenInfo, getCursorPtr));
	printf("offsetof(rfbScreenInfo, setTranslateFunction) : %zu\n", offsetof(rfbScreenInfo, setTranslateFunction));
	printf("offsetof(rfbScreenInfo, setSingleWindow) : %zu\n", offsetof(rfbScreenInfo, setSingleWindow));
	printf("offsetof(rfbScreenInfo, setServerInput) : %zu\n", offsetof(rfbScreenInfo, setServerInput));
	printf("offsetof(rfbScreenInfo, getFileTransferPermission) : %zu\n", offsetof(rfbScreenInfo, getFileTransferPermission));
	printf("offsetof(rfbScreenInfo, setTextChat) : %zu\n", offsetof(rfbScreenInfo, setTextChat));

	printf("offsetof(rfbScreenInfo, newClientHook) : %zu\n", offsetof(rfbScreenInfo, newClientHook));
	printf("offsetof(rfbScreenInfo, displayHook) : %zu\n", offsetof(rfbScreenInfo, displayHook));

	printf("offsetof(rfbScreenInfo, getKeyboardLedStateHook) : %zu\n", offsetof(rfbScreenInfo, getKeyboardLedStateHook));

#if defined(LIBVNCSERVER_HAVE_LIBPTHREAD) || defined(LIBVNCSERVER_HAVE_WIN32THREADS)
	printf("offsetof(rfbScreenInfo, cursorMutex) : %zu\n", offsetof(rfbScreenInfo, cursorMutex));
	printf("offsetof(rfbScreenInfo, backgroundLoop) : %zu\n", offsetof(rfbScreenInfo, backgroundLoop));
#endif

	printf("offsetof(rfbScreenInfo, ignoreSIGPIPE) : %zu\n", offsetof(rfbScreenInfo, ignoreSIGPIPE));

	printf("offsetof(rfbScreenInfo, progressiveSliceHeight) : %zu\n", offsetof(rfbScreenInfo, progressiveSliceHeight));

	printf("offsetof(rfbScreenInfo, listenInterface) : %zu\n", offsetof(rfbScreenInfo, listenInterface));
	printf("offsetof(rfbScreenInfo, deferPtrUpdateTime) : %zu\n", offsetof(rfbScreenInfo, deferPtrUpdateTime));

	printf("offsetof(rfbScreenInfo, handleEventsEagerly) : %zu\n", offsetof(rfbScreenInfo, handleEventsEagerly));

	printf("offsetof(rfbScreenInfo, versionString) : %zu\n", offsetof(rfbScreenInfo, versionString));

	printf("offsetof(rfbScreenInfo, protocolMajorVersion) : %zu\n", offsetof(rfbScreenInfo, protocolMajorVersion));
	printf("offsetof(rfbScreenInfo, protocolMinorVersion) : %zu\n", offsetof(rfbScreenInfo, protocolMinorVersion));

	printf("offsetof(rfbScreenInfo, permitFileTransfer) : %zu\n", offsetof(rfbScreenInfo, permitFileTransfer));

	printf("offsetof(rfbScreenInfo, displayFinishedHook) : %zu\n", offsetof(rfbScreenInfo, displayFinishedHook));
	printf("offsetof(rfbScreenInfo, xvpHooka) : %zu\n", offsetof(rfbScreenInfo, xvpHook));
	printf("offsetof(rfbScreenInfo, sslkeyfile) : %zu\n", offsetof(rfbScreenInfo, sslkeyfile));
	printf("offsetof(rfbScreenInfo, sslcertfile) : %zu\n", offsetof(rfbScreenInfo, sslcertfile));
	printf("offsetof(rfbScreenInfo, ipv6port) : %zu\n", offsetof(rfbScreenInfo, ipv6port));
	printf("offsetof(rfbScreenInfo, listen6Interface) : %zu\n", offsetof(rfbScreenInfo, listen6Interface));

	printf("offsetof(rfbScreenInfo, listen6Sock) : %zu\n", offsetof(rfbScreenInfo, listen6Sock));
	printf("offsetof(rfbScreenInfo, http6Port) : %zu\n", offsetof(rfbScreenInfo, http6Port));
	printf("offsetof(rfbScreenInfo, httpListen6Sock) : %zu\n", offsetof(rfbScreenInfo, httpListen6Sock));

#if __linux__
	// Ubuntu ships with 0.9.12, which does not have these values
#else
	printf("offsetof(rfbScreenInfo, setDesktopSizeHook) : %zu\n", offsetof(rfbScreenInfo, setDesktopSizeHook));
	printf("offsetof(rfbScreenInfo, numberOfExtDesktopScreensHook) : %zu\n", offsetof(rfbScreenInfo, numberOfExtDesktopScreensHook));
	printf("offsetof(rfbScreenInfo, getExtDesktopScreenHook) : %zu\n", offsetof(rfbScreenInfo, getExtDesktopScreenHook));
	printf("offsetof(rfbScreenInfo, fdQuota) : %zu\n", offsetof(rfbScreenInfo, fdQuota));
#endif

	/** back pointer to the screen */
	printf("offsetof(rfbClientRec, screen) : %zu\n", offsetof(rfbClientRec, screen));
	printf("offsetof(rfbClientRec, scaledScreen) : %zu\n", offsetof(rfbClientRec, scaledScreen));
	printf("offsetof(rfbClientRec, PalmVNC) : %zu\n", offsetof(rfbClientRec, PalmVNC));
	printf("offsetof(rfbClientRec, clientData) : %zu\n", offsetof(rfbClientRec, clientData));
	printf("offsetof(rfbClientRec, clientGoneHook) : %zu\n", offsetof(rfbClientRec, clientGoneHook));
	printf("offsetof(rfbClientRec, sock) : %zu\n", offsetof(rfbClientRec, sock));
	printf("offsetof(rfbClientRec, host) : %zu\n", offsetof(rfbClientRec, host));
	printf("offsetof(rfbClientRec, protocolMajorVersion) : %zu\n", offsetof(rfbClientRec, protocolMajorVersion));
	printf("offsetof(rfbClientRec, protocolMinorVersion) : %zu\n", offsetof(rfbClientRec, protocolMinorVersion));

#if defined(LIBVNCSERVER_HAVE_LIBPTHREAD) || defined(LIBVNCSERVER_HAVE_WIN32THREADS)
	printf("offsetof(rfbClientRec, client_thread) : %zu\n", offsetof(rfbClientRec, client_thread));
#endif
	printf("offsetof(rfbClientRec, state) : %zu\n", offsetof(rfbClientRec, state));

	printf("offsetof(rfbClientRec, reverseConnection) : %zu\n", offsetof(rfbClientRec, reverseConnection));
	printf("offsetof(rfbClientRec, onHold) : %zu\n", offsetof(rfbClientRec, onHold));
	printf("offsetof(rfbClientRec, readyForSetColourMapEntries) : %zu\n", offsetof(rfbClientRec, readyForSetColourMapEntries));
	printf("offsetof(rfbClientRec, useCopyRect) : %zu\n", offsetof(rfbClientRec, useCopyRect));
	printf("offsetof(rfbClientRec, preferredEncoding) : %zu\n", offsetof(rfbClientRec, preferredEncoding));
	printf("offsetof(rfbClientRec, correMaxWidth) : %zu\n", offsetof(rfbClientRec, correMaxWidth));
	printf("offsetof(rfbClientRec, correMaxHeight) : %zu\n", offsetof(rfbClientRec, correMaxHeight));
	printf("offsetof(rfbClientRec, viewOnly) : %zu\n", offsetof(rfbClientRec, viewOnly));
	printf("offsetof(rfbClientRec, authChallenge) : %zu\n", offsetof(rfbClientRec, authChallenge));
	printf("offsetof(rfbClientRec, copyRegion) : %zu\n", offsetof(rfbClientRec, copyRegion));
	printf("offsetof(rfbClientRec, copyDX) : %zu\n", offsetof(rfbClientRec, copyDX));
	printf("offsetof(rfbClientRec, copyDY) : %zu\n", offsetof(rfbClientRec, copyDY));
	printf("offsetof(rfbClientRec, modifiedRegion) : %zu\n", offsetof(rfbClientRec, modifiedRegion));
	printf("offsetof(rfbClientRec, requestedRegion) : %zu\n", offsetof(rfbClientRec, requestedRegion));
	printf("offsetof(rfbClientRec, startDeferring) : %zu\n", offsetof(rfbClientRec, startDeferring));
	printf("offsetof(rfbClientRec, startPtrDeferring) : %zu\n", offsetof(rfbClientRec, startPtrDeferring));
	printf("offsetof(rfbClientRec, lastPtrX) : %zu\n", offsetof(rfbClientRec, lastPtrX));
	printf("offsetof(rfbClientRec, lastPtrY) : %zu\n", offsetof(rfbClientRec, lastPtrY));
	printf("offsetof(rfbClientRec, lastPtrButtons) : %zu\n", offsetof(rfbClientRec, lastPtrButtons));
	printf("offsetof(rfbClientRec, translateFn) : %zu\n", offsetof(rfbClientRec, translateFn));
	printf("offsetof(rfbClientRec, translateLookupTable) : %zu\n", offsetof(rfbClientRec, translateLookupTable));
	printf("offsetof(rfbClientRec, format) : %zu\n", offsetof(rfbClientRec, format));
	printf("offsetof(rfbClientRec, updateBuf) : %zu\n", offsetof(rfbClientRec, updateBuf));
	printf("offsetof(rfbClientRec, ublen) : %zu\n", offsetof(rfbClientRec, ublen));
	printf("offsetof(rfbClientRec, statEncList) : %zu\n", offsetof(rfbClientRec, statEncList));
	printf("offsetof(rfbClientRec, statMsgList) : %zu\n", offsetof(rfbClientRec, statMsgList));
	printf("offsetof(rfbClientRec, rawBytesEquivalent) : %zu\n", offsetof(rfbClientRec, rawBytesEquivalent));
	printf("offsetof(rfbClientRec, bytesSent) : %zu\n", offsetof(rfbClientRec, bytesSent));
#ifdef LIBVNCSERVER_HAVE_LIBZ
	printf("offsetof(rfbClientRec, compStream) : %zu\n", offsetof(rfbClientRec, compStream));
	printf("offsetof(rfbClientRec, compStreamInited) : %zu\n", offsetof(rfbClientRec, compStreamInited));
	printf("offsetof(rfbClientRec, zlibCompressLevel) : %zu\n", offsetof(rfbClientRec, zlibCompressLevel));
#endif
#if defined(LIBVNCSERVER_HAVE_LIBZ) || defined(LIBVNCSERVER_HAVE_LIBPNG)
	printf("offsetof(rfbClientRec, tightQualityLevel) : %zu\n", offsetof(rfbClientRec, tightQualityLevel));

#ifdef LIBVNCSERVER_HAVE_LIBJPEG
	printf("offsetof(rfbClientRec, zsStruct) : %zu\n", offsetof(rfbClientRec, zsStruct));
	printf("offsetof(rfbClientRec, zsActive) : %zu\n", offsetof(rfbClientRec, zsActive));
	printf("offsetof(rfbClientRec, zsLevel) : %zu\n", offsetof(rfbClientRec, zsLevel));
	printf("offsetof(rfbClientRec, tightCompressLevel) : %zu\n", offsetof(rfbClientRec, tightCompressLevel));
#endif
#endif
	printf("offsetof(rfbClientRec, compStreamInitedLZO) : %zu\n", offsetof(rfbClientRec, compStreamInitedLZO));
	printf("offsetof(rfbClientRec, lzoWrkMem) : %zu\n", offsetof(rfbClientRec, lzoWrkMem));
	printf("offsetof(rfbClientRec, fileTransfer) : %zu\n", offsetof(rfbClientRec, fileTransfer));
	printf("offsetof(rfbClientRec, lastKeyboardLedState) : %zu\n", offsetof(rfbClientRec, lastKeyboardLedState));
	printf("offsetof(rfbClientRec, enableSupportedMessages) : %zu\n", offsetof(rfbClientRec, enableSupportedMessages));
	printf("offsetof(rfbClientRec, enableSupportedEncodings) : %zu\n", offsetof(rfbClientRec, enableSupportedEncodings));
	printf("offsetof(rfbClientRec, enableServerIdentity) : %zu\n", offsetof(rfbClientRec, enableServerIdentity));
	printf("offsetof(rfbClientRec, enableKeyboardLedState) : %zu\n", offsetof(rfbClientRec, enableKeyboardLedState));
	printf("offsetof(rfbClientRec, enableLastRectEncoding) : %zu\n", offsetof(rfbClientRec, enableLastRectEncoding));
	printf("offsetof(rfbClientRec, enableCursorShapeUpdates) : %zu\n", offsetof(rfbClientRec, enableCursorShapeUpdates));
	printf("offsetof(rfbClientRec, enableCursorPosUpdates) : %zu\n", offsetof(rfbClientRec, enableCursorPosUpdates));
	printf("offsetof(rfbClientRec, useRichCursorEncoding) : %zu\n", offsetof(rfbClientRec, useRichCursorEncoding));
	printf("offsetof(rfbClientRec, cursorWasChanged) : %zu\n", offsetof(rfbClientRec, cursorWasChanged));
	printf("offsetof(rfbClientRec, cursorWasMoved) : %zu\n", offsetof(rfbClientRec, cursorWasMoved));
	printf("offsetof(rfbClientRec, cursorX) : %zu\n", offsetof(rfbClientRec, cursorX));
	printf("offsetof(rfbClientRec, cursorY) : %zu\n", offsetof(rfbClientRec, cursorY));
	printf("offsetof(rfbClientRec, useNewFBSize) : %zu\n", offsetof(rfbClientRec, useNewFBSize));
	printf("offsetof(rfbClientRec, newFBSizePending) : %zu\n", offsetof(rfbClientRec, newFBSizePending));
	printf("offsetof(rfbClientRec, prev) : %zu\n", offsetof(rfbClientRec, prev));
	printf("offsetof(rfbClientRec, next) : %zu\n", offsetof(rfbClientRec, next));
#if defined(LIBVNCSERVER_HAVE_LIBPTHREAD) || defined(LIBVNCSERVER_HAVE_WIN32THREADS)
	printf("offsetof(rfbClientRec, refCount) : %zu\n", offsetof(rfbClientRec, refCount));
	printf("offsetof(rfbClientRec, refCountMutex) : %zu\n", offsetof(rfbClientRec, refCountMutex));
	printf("offsetof(rfbClientRec, deleteCond) : %zu\n", offsetof(rfbClientRec, deleteCond));

	printf("offsetof(rfbClientRec, outputMutex) : %zu\n", offsetof(rfbClientRec, outputMutex));
	printf("offsetof(rfbClientRec, updateMutex) : %zu\n", offsetof(rfbClientRec, updateMutex));
	printf("offsetof(rfbClientRec, updateCond) : %zu\n", offsetof(rfbClientRec, updateCond));
#endif

#ifdef LIBVNCSERVER_HAVE_LIBZ
	printf("offsetof(rfbClientRec, zrleData) : %zu\n", offsetof(rfbClientRec, zrleData));
	printf("offsetof(rfbClientRec, zywrleLevel) : %zu\n", offsetof(rfbClientRec, zywrleLevel));
	printf("offsetof(rfbClientRec, zywrleBuf) : %zu\n", offsetof(rfbClientRec, zywrleBuf));
#endif

	printf("offsetof(rfbClientRec, progressiveSliceY) : %zu\n", offsetof(rfbClientRec, progressiveSliceY));
	printf("offsetof(rfbClientRec, extensions) : %zu\n", offsetof(rfbClientRec, extensions));
	printf("offsetof(rfbClientRec, zrleBeforeBuf) : %zu\n", offsetof(rfbClientRec, zrleBeforeBuf));
	printf("offsetof(rfbClientRec, paletteHelper) : %zu\n", offsetof(rfbClientRec, paletteHelper));

#if defined(LIBVNCSERVER_HAVE_LIBPTHREAD) || defined(LIBVNCSERVER_HAVE_WIN32THREADS)
#define LIBVNCSERVER_SEND_MUTEX
	printf("offsetof(rfbClientRec, sendMutex) : %zu\n", offsetof(rfbClientRec, sendMutex));
#endif
	printf("offsetof(rfbClientRec, beforeEncBuf) : %zu\n", offsetof(rfbClientRec, beforeEncBuf));
	printf("offsetof(rfbClientRec, beforeEncBufSize) : %zu\n", offsetof(rfbClientRec, beforeEncBufSize));
	printf("offsetof(rfbClientRec, afterEncBuf) : %zu\n", offsetof(rfbClientRec, afterEncBuf));
	printf("offsetof(rfbClientRec, afterEncBufSize) : %zu\n", offsetof(rfbClientRec, afterEncBufSize));
	printf("offsetof(rfbClientRec, afterEncBufLen) : %zu\n", offsetof(rfbClientRec, afterEncBufLen));
#if defined(LIBVNCSERVER_HAVE_LIBZ) || defined(LIBVNCSERVER_HAVE_LIBPNG)
	printf("offsetof(rfbClientRec, tightEncoding) : %zu\n", offsetof(rfbClientRec, tightEncoding));
#ifdef LIBVNCSERVER_HAVE_LIBJPEG
	printf("offsetof(rfbClientRec, turboSubsampLevel) : %zu\n", offsetof(rfbClientRec, turboSubsampLevel));
	printf("offsetof(rfbClientRec, turboQualityLevel) : %zu\n", offsetof(rfbClientRec, turboQualityLevel));
#endif
#endif
	printf("offsetof(rfbClientRec, sslctx) : %zu\n", offsetof(rfbClientRec, sslctx));
	printf("offsetof(rfbClientRec, wsctx) : %zu\n", offsetof(rfbClientRec, wsctx));
	printf("offsetof(rfbClientRec, wspath) : %zu\n", offsetof(rfbClientRec, wspath));
#if __linux__
	// Ubuntu ships with 0.9.12, which does not have these values
#else
#ifdef LIBVNCSERVER_HAVE_LIBPTHREAD
	printf("offsetof(rfbClientRec, pipe_notify_client_thread) : %zu\n", offsetof(rfbClientRec, pipe_notify_client_thread));
#endif
	printf("offsetof(rfbClientRec, clientFramebufferUpdateRequestHook) : %zu\n", offsetof(rfbClientRec, clientFramebufferUpdateRequestHook));
	printf("offsetof(rfbClientRec, useExtDesktopSize) : %zu\n", offsetof(rfbClientRec, useExtDesktopSize));
	printf("offsetof(rfbClientRec, requestedDesktopSizeChange) : %zu\n", offsetof(rfbClientRec, requestedDesktopSizeChange));
	printf("offsetof(rfbClientRec, lastDesktopSizeChangeError) : %zu\n", offsetof(rfbClientRec, lastDesktopSizeChangeError));
#endif
	return 0;
}