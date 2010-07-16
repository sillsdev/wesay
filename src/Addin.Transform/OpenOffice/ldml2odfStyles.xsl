<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
				xmlns:office="urn:oasis:names:tc:opendocument:xmlns:office:1.0"
				xmlns:style="urn:oasis:names:tc:opendocument:xmlns:style:1.0"
				xmlns:text="urn:oasis:names:tc:opendocument:xmlns:text:1.0"
				xmlns:table="urn:oasis:names:tc:opendocument:xmlns:table:1.0"
				xmlns:draw="urn:oasis:names:tc:opendocument:xmlns:drawing:1.0"
				xmlns:fo="urn:oasis:names:tc:opendocument:xmlns:xsl-fo-compatible:1.0"
				xmlns:xlink="http://www.w3.org/1999/xlink"
				xmlns:dc="http://purl.org/dc/elements/1.1/"
				xmlns:meta="urn:oasis:names:tc:opendocument:xmlns:meta:1.0"
				xmlns:number="urn:oasis:names:tc:opendocument:xmlns:datastyle:1.0"
				xmlns:svg="urn:oasis:names:tc:opendocument:xmlns:svg-compatible:1.0"
				xmlns:chart="urn:oasis:names:tc:opendocument:xmlns:chart:1.0"
				xmlns:dr3d="urn:oasis:names:tc:opendocument:xmlns:dr3d:1.0"
				xmlns:math="http://www.w3.org/1998/Math/MathML"
				xmlns:form="urn:oasis:names:tc:opendocument:xmlns:form:1.0"
				xmlns:script="urn:oasis:names:tc:opendocument:xmlns:script:1.0"
				xmlns:ooo="http://openoffice.org/2004/office"
				xmlns:ooow="http://openoffice.org/2004/writer"
				xmlns:oooc="http://openoffice.org/2004/calc"
				xmlns:dom="http://www.w3.org/2001/xml-events"
				xmlns:xforms="http://www.w3.org/2002/xforms"
				xmlns:xsd="http://www.w3.org/2001/XMLSchema"
				xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
				xmlns:rpt="http://openoffice.org/2005/report"
				xmlns:of="urn:oasis:names:tc:opendocument:xmlns:of:1.2"
				xmlns:rdfa="http://docs.oasis-open.org/opendocument/meta/rdfa#"
				xmlns:field="urn:openoffice:names:experimental:ooo-ms-interop:xmlns:field:1.0"
				xmlns:palaso="urn://palaso.org/ldmlExtensions/v1"
				version="1.0"
				office:version="1.2"
				exclude-result-prefixes="xsl">
	<xsl:param name="primaryLangCode" select="'th'"/>

	<xsl:variable name="primaryFont"
				  select="/root/ldml/special[palaso:abbreviation/@value=$primaryLangCode]/palaso:defaultFontFamily/@value"/>
	<xsl:strip-space elements="text:p text:span"/>
	<!-- indent is nice for debugging, but it introduces stray spaces
The xsl:text elements below are used to introduce line breaks
in harmless places when there is no indentation.
 -->
	<xsl:output method="xml"
				omit-xml-declaration="no"
				indent="no"/>
	<!--
styles to support:
lexical-unit
citation
pronunciation
variant
	pronunciation
	relation
sense
	grammatical-info
	gloss
	note
	example
		translation
note
relation? - entry/sense crossreference
etymology
	gloss

-->
	<xsl:template match="/">
		<office:document-styles>
			<office:font-face-decls>
				<!-- Include each font only once -->
				<xsl:for-each select="//palaso:defaultFontFamily">
					<xsl:variable name="font"
								  select="@value"/>
					<xsl:variable name="check" select="//palaso:defaultFontFamily[@value = $font]"/>
					<xsl:variable name="pos">
						<xsl:number count="palaso:defaultFontFamily[@value = $font]"
									level="any"/>
					</xsl:variable>
					<xsl:if test="$pos = 1">
						<style:font-face style:name="{@value}"
										 svg:font-family="{@value}"/>
					</xsl:if>
				</xsl:for-each>
			</office:font-face-decls>
			<xsl:text>
			</xsl:text>
			<office:styles>
				<style:style style:name="Standard"
							 style:family="paragraph"
							 style:class="text"/>
				<xsl:text>
			</xsl:text>
				<style:style style:name="Text_20_body"
							 style:display-name="Text body"
							 style:family="paragraph"
							 style:parent-style-name="Standard"
							 style:class="text">
					<style:paragraph-properties fo:margin-top="0cm"
												fo:margin-bottom="0.1cm"/>
					<style:text-properties fo:font-size="12pt"
										   style:font-size-asian="12pt"
										   style:font-size-complex="12pt"
										   fo:font-weight="normal"
										   style:font-weight-asian="normal"
										   style:font-weight-complex="normal"
										   style:font-name="{$primaryFont}"
										   style:font-name-asian="{$primaryFont}"
										   style:font-name-complex="{$primaryFont}"/>
				</style:style>
				<xsl:text>
			</xsl:text>
				<style:style style:name="Heading"
							 style:family="paragraph"
							 style:parent-style-name="Standard"
							 style:next-style-name="Text_20_body"
							 style:class="text">
					<style:paragraph-properties fo:margin-top="0.423cm"
												fo:margin-bottom="0.212cm"
												fo:keep-with-next="always"/>
					<style:text-properties fo:font-size="14pt"
										   style:font-size-asian="14pt"
										   style:font-size-complex="14pt"
										   fo:font-weight="normal"
										   style:font-weight-asian="normal"
										   style:font-weight-complex="normal"
										   style:font-name="{$primaryFont}"
										   style:font-name-asian="{$primaryFont}"
										   style:font-name-complex="{$primaryFont}"/>
				</style:style>
				<xsl:text>
			</xsl:text>
				<style:style style:name="Title"
							 style:family="paragraph"
							 style:parent-style-name="Heading"
							 style:next-style-name="Subtitle"
							 style:class="chapter">
					<style:paragraph-properties fo:text-align="center"
												style:justify-single-word="false"/>
					<style:text-properties fo:font-size="18pt"
										   fo:font-weight="bold"
										   style:font-size-asian="18pt"
										   style:font-weight-asian="bold"
										   style:font-size-complex="18pt"
										   style:font-weight-complex="bold"/>
				</style:style>
				<xsl:text>
			</xsl:text>
				<style:style style:name="Subtitle"
							 style:family="paragraph"
							 style:parent-style-name="Heading"
							 style:next-style-name="Text_20_body"
							 style:class="chapter">
					<style:paragraph-properties fo:text-align="center"
												style:justify-single-word="false"/>
					<style:text-properties fo:font-size="14pt"
										   fo:font-style="italic"
										   style:font-size-asian="14pt"
										   style:font-style-asian="italic"
										   style:font-size-complex="14pt"
										   style:font-style-complex="italic"/>
				</style:style>
				<xsl:text>
			</xsl:text>
				<style:style style:name="Heading_20_1"
							 style:display-name="Heading 1"
							 style:family="paragraph"
							 style:parent-style-name="Heading"
							 style:next-style-name="Text_20_body"
							 style:default-outline-level="1"
							 style:class="text">
					<style:text-properties fo:font-size="115%"
										   fo:font-weight="bold"
										   style:font-size-asian="115%"
										   style:font-weight-asian="bold"
										   style:font-size-complex="115%"
										   style:font-weight-complex="bold"
										   style:font-name="{$primaryFont}"
										   style:font-name-asian="{$primaryFont}"
										   style:font-name-complex="{$primaryFont}"/>
				</style:style>
				<xsl:text>
			</xsl:text>
				<style:style style:name="Header"
							 style:family="paragraph"
							 style:parent-style-name="Standard"
							 style:class="extra">
					<style:paragraph-properties text:number-lines="false"
												text:line-number="0"
												fo:text-align="left">
						<style:tab-stops>
							<style:tab-stop style:position="8.5cm"
											style:type="center"/>
							<style:tab-stop style:position="17cm"
											style:type="right"/>
						</style:tab-stops>
					</style:paragraph-properties>
					<style:text-properties fo:font-size="10pt"
										   fo:font-weight="bold"
										   style:font-size-asian="10pt"
										   style:font-weight-asian="bold"
										   style:font-size-complex="10pt"
										   style:font-weight-complex="bold"
										   style:font-name="{$primaryFont}"
										   style:font-name-asian="{$primaryFont}"
										   style:font-name-complex="{$primaryFont}"/>
				</style:style>
				<xsl:text>
			</xsl:text>
				<style:style style:name="HeaderFrame"
							 style:family="paragraph"
							 style:parent-style-name="Header"
							 style:class="extra">
					<style:paragraph-properties text:number-lines="false"
												text:line-number="0"
												fo:text-align="right">
					</style:paragraph-properties>
					<style:text-properties fo:font-size="10pt"
										   fo:font-weight="bold"
										   style:font-size-asian="10pt"
										   style:font-weight-asian="bold"
										   style:font-size-complex="10pt"
										   style:font-weight-complex="bold"
										   style:font-name="{$primaryFont}"
										   style:font-name-asian="{$primaryFont}"
										   style:font-name-complex="{$primaryFont}"/>
				</style:style>
				<xsl:text>
			</xsl:text>
				<style:style style:name="entry"
							 style:display-name="entry"
							 style:family="paragraph"
							 style:parent-style-name="Text_20_body"
							 style:class="text">
					<style:paragraph-properties fo:margin-top="0cm"
												fo:margin-bottom="0.1cm"
												fo:orphans="2"
												fo:widows="2"/>
				</style:style>
				<xsl:text>
			 </xsl:text>
				<style:style style:name="Illustration"
							 style:display-name="Illustration"
							 style:family="paragraph"
							 style:parent-style-name="Text_20_body"
							 style:next-style-name="Illustration"
							 style:class="text">
					<style:paragraph-properties fo:text-align="center"/>
					<style:text-properties style:font-name="{$primaryFont}"
										   style:font-name-asian="{$primaryFont}"
										   style:font-name-complex="{$primaryFont}"
										   fo:font-style="italic"
										   style:font-style-asian="italic"
										   style:font-style-complex="italic"/>
				</style:style>
				<xsl:text>
			</xsl:text>
				<style:style style:name="sense-number"
							 style:display-name="sense-number"
							 style:family="text">
					<style:text-properties style:font-name="{$primaryFont}"
										   style:font-name-asian="{$primaryFont}"
										   style:font-name-complex="{$primaryFont}"
										   fo:font-size="10pt"
										   style:font-size-asian="10pt"
										   style:font-size-complex="10pt"
										   style:text-position="super"/>
				</style:style>
				<xsl:text>
			 </xsl:text>
				<style:style style:name="grammatical-info"
							 style:display-name="grammatical-info"
							 style:family="text">
					<style:text-properties style:font-name="Arial"
										   style:font-name-asian="Tahoma"
										   style:font-name-complex="Tahoma"
										   fo:font-size="10pt"
										   style:font-size-asian="10pt"
										   style:font-size-complex="10pt"
										   fo:font-variant="small-caps"/>
				</style:style>
				<xsl:text>
			 </xsl:text>
				<style:style style:name="lexical-unit"
							 style:display-name="lexical-unit"
							 style:family="text">
					<style:text-properties style:font-name="Times New Roman"
										   style:font-name-asian="Tahoma"
										   style:font-name-complex="Tahoma"
										   fo:font-weight="bold"
										   style:font-weight-asian="bold"
										   style:font-weight-complex="bold"/>
				</style:style>
				<xsl:text>
			 </xsl:text>
				<xsl:call-template name="text-style">
					<xsl:with-param name="lift-element">lexical-unit</xsl:with-param>
				</xsl:call-template>
				<style:style style:name="header-word"
							 style:display-name="header-word"
							 style:family="text">
					<style:text-properties style:font-name="Times New Roman"
										   style:font-name-asian="Tahoma"
										   style:font-name-complex="Tahoma"
										   fo:font-weight="bold"
										   style:font-weight-asian="bold"
										   style:font-weight-complex="bold"/>
				</style:style>
				<xsl:text>
			 </xsl:text>
				<xsl:call-template name="text-style">
					<xsl:with-param name="lift-element">header-word</xsl:with-param>
				</xsl:call-template>
				<style:style style:name="definition"
							 style:display-name="definition"
							 style:family="text">
					<style:text-properties style:font-name="Times New Roman"
										   style:font-name-asian="Tahoma"
										   style:font-name-complex="Tahoma"/>
				</style:style>
				<xsl:text>
			 </xsl:text>
				<xsl:call-template name="text-style">
					<xsl:with-param name="lift-element">definition</xsl:with-param>
				</xsl:call-template>
				<style:style style:name="citation"
							 style:display-name="citation"
							 style:family="text">
					<style:text-properties style:font-name="Times New Roman"
										   style:font-name-asian="Tahoma"
										   style:font-name-complex="Tahoma"/>
				</style:style>
				<xsl:text>
			 </xsl:text>
				<xsl:call-template name="text-style">
					<xsl:with-param name="lift-element">citation</xsl:with-param>
				</xsl:call-template>
				<style:style style:name="pronunciation"
							 style:display-name="pronunciation"
							 style:family="text">
					<style:text-properties style:font-name="Times New Roman"
										   style:font-name-asian="Tahoma"
										   style:font-name-complex="Tahoma"
										   fo:font-style="italic"
										   style:font-style-asian="italic"
										   style:font-style-complex="italic"/>
				</style:style>
				<xsl:text>
			 </xsl:text>
				<xsl:call-template name="text-style">
					<xsl:with-param name="lift-element">pronunciation</xsl:with-param>
				</xsl:call-template>
				<style:style style:name="sense"
							 style:display-name="sense"
							 style:family="text">
					<style:text-properties style:font-name="Times New Roman"
										   style:font-name-asian="Tahoma"
										   style:font-name-complex="Tahoma"/>
				</style:style>
				<xsl:text>
			 </xsl:text>
				<xsl:call-template name="text-style">
					<xsl:with-param name="lift-element">sense</xsl:with-param>
				</xsl:call-template>
				<style:style style:name="gloss"
							 style:display-name="gloss"
							 style:family="text">
					<style:text-properties style:font-name="Times New Roman"
										   style:font-name-asian="Tahoma"
										   style:font-name-complex="Tahoma"/>
				</style:style>
				<xsl:text>
			 </xsl:text>
				<xsl:call-template name="text-style">
					<xsl:with-param name="lift-element">gloss</xsl:with-param>
				</xsl:call-template>
				<style:style style:name="note"
							 style:display-name="note"
							 style:family="text">
					<style:text-properties style:font-name="Times New Roman"
										   style:font-name-asian="Tahoma"
										   style:font-name-complex="Tahoma"/>
				</style:style>
				<xsl:text>
			 </xsl:text>
				<xsl:call-template name="text-style">
					<xsl:with-param name="lift-element">note</xsl:with-param>
				</xsl:call-template>
				<style:style style:name="example"
							 style:display-name="example"
							 style:family="text">
					<style:text-properties style:font-name="Times New Roman"
										   style:font-name-asian="Tahoma"
										   style:font-name-complex="Tahoma"/>
				</style:style>
				<xsl:text>
			 </xsl:text>
				<xsl:call-template name="text-style">
					<xsl:with-param name="lift-element">example</xsl:with-param>
				</xsl:call-template>
				<style:style style:name="translation"
							 style:display-name="translation"
							 style:family="text">
					<style:text-properties style:font-name="Times New Roman"
										   style:font-name-asian="Tahoma"
										   style:font-name-complex="Tahoma"/>
				</style:style>
				<xsl:text>
			 </xsl:text>
				<xsl:call-template name="text-style">
					<xsl:with-param name="lift-element">translation</xsl:with-param>
				</xsl:call-template>
				<style:style style:name="etymology"
							 style:display-name="etymology"
							 style:family="text">
					<style:text-properties style:font-name="Times New Roman"
										   style:font-name-asian="Tahoma"
										   style:font-name-complex="Tahoma"/>
				</style:style>
				<xsl:text>
			 </xsl:text>
				<xsl:call-template name="text-style">
					<xsl:with-param name="lift-element">etymology</xsl:with-param>
				</xsl:call-template>
				<style:style style:name="label"
							 style:display-name="label"
							 style:family="text">
					<style:text-properties style:font-name="Times New Roman"
										   style:font-name-asian="Tahoma"
										   style:font-name-complex="Tahoma"
										   style:text-underline-style="solid"
										   style:text-underline-width="auto"
										   style:text-underline-color="font-color"/>
				</style:style>
				<xsl:text>
			 </xsl:text>
				<xsl:call-template name="text-style">
					<xsl:with-param name="lift-element">label</xsl:with-param>
				</xsl:call-template>
				<style:style style:name="lastEntryHeader"
							 style:family="graphic"
							 style:parent-style-name="Frame">
					<style:graphic-properties style:vertical-pos="from-top"
											  style:vertical-rel="page"
											  style:horizontal-pos="from-left"
											  style:horizontal-rel="page"
											  fo:padding="0cm"
											  fo:border="none"
											  style:shadow="none"
											  style:flow-with-text="false"/>
				</style:style>
				<style:style style:name="Illustration_Caption"
							 style:family="graphic">
					<style:graphic-properties style:run-through="background"
											  style:wrap="dynamic"
											  style:number-wrapped-paragraphs="no-limit"
											  style:vertical-pos="from-top"
											  style:vertical-rel="paragraph"
											  style:horizontal-pos="right"
											  style:horizontal-rel="paragraph"
											  fo:background-color="transparent"
											  style:background-transparency="100%"
											  fo:padding="0cm"
											  fo:border="none"
											  fo:margin-left="0cm"
											  fo:margin-right="0cm"
											  fo:margin-top="0cm"
											  fo:margin-bottom="0cm"
											  fo:min-width="2.5cm">
					</style:graphic-properties>
				</style:style>
				<style:style style:name="Illustration"
							 style:family="graphic">
					<style:graphic-properties style:run-through="background"
											  style:wrap="none"
											  style:number-wrapped-paragraphs="no-limit"
											  style:vertical-pos="from-top"
											  style:vertical-rel="paragraph"
											  style:horizontal-pos="center"
											  style:horizontal-rel="paragraph-content"
											  fo:background-color="transparent"
											  style:background-transparency="100%"
											  fo:padding="0cm"
											  fo:border="none"
											  fo:margin-left="0cm"
											  fo:margin-right="0cm"
											  fo:margin-top="0cm"
											  fo:margin-bottom="0cm"
											  draw:image-opacity="100%">
					</style:graphic-properties>
				</style:style>
			</office:styles>
			<xsl:text>
			 </xsl:text>
			<office:automatic-styles>
				<style:page-layout style:name="Mpm1">
					<style:page-layout-properties fo:page-width="21.001cm"
												  fo:page-height="29.7cm"
												  style:num-format="1"
												  style:print-orientation="portrait"
												  fo:margin-top="2cm"
												  fo:margin-bottom="2cm"
												  fo:margin-left="2cm"
												  fo:margin-right="2cm"
												  style:writing-mode="lr-tb"
												  style:footnote-max-height="0cm">
						<style:footnote-sep style:width="0.018cm"
											style:distance-before-sep="0.101cm"
											style:distance-after-sep="0.101cm"
											style:adjustment="left"
											style:rel-width="25%"
											style:color="#000000"/>
					</style:page-layout-properties>
					<style:header-style>
						<style:header-footer-properties fo:min-height="0cm"
														fo:margin-bottom="0.499cm"/>
					</style:header-style>
					<style:footer-style/>
				</style:page-layout>
			</office:automatic-styles>
			<xsl:text>
			</xsl:text>
			<office:master-styles>
				<style:master-page style:name="Standard"
								   style:page-layout-name="Mpm1">
					<style:header>
						<text:p text:style-name="Header"
								xml:space="default">
							<xsl:choose>
								<xsl:when test="string-length($primaryLangCode) &gt; 0">
									<text:span text:style-name="{concat('header-word_', $primaryLangCode)}">
										<text:variable-get text:name="EntryWord"
														   office:value-type="string"/>
									</text:span>
									<text:tab/>
									<text:page-number text:select-page="current"
													  style:num-format="Native Numbering">1</text:page-number>
								</xsl:when>
								<xsl:otherwise>
									<text:variable-get text:name="EntryWord"
													   office:value-type="string"/>
									<text:tab/>
									<text:page-number text:select-page="current"
													  style:num-format="Native Numbering">1</text:page-number>
								</xsl:otherwise>
							</xsl:choose>
						</text:p>
					</style:header>
					<style:footer>
						<text:p text:style-name="Header"
								xml:space="default">
							<draw:frame draw:style-name="lastEntryHeader"
										draw:name="FooterLastEntryFrame"
										text:anchor-type="paragraph"
										svg:x="14cm"
										svg:y="2cm"
										svg:width="5cm"
										draw:z-index="0">
								<draw:text-box fo:min-height="0.5cm">
									<text:p text:style-name="HeaderFrame"
											xml:space="default">
										<xsl:choose>
											<xsl:when test="string-length($primaryLangCode) &gt; 0">
												<text:span text:style-name="{concat('header-word_', $primaryLangCode)}">
													<text:variable-get text:name="EntryWord"
																	   office:value-type="string"/>
												</text:span>
											</xsl:when>
											<xsl:otherwise>
												<text:variable-get text:name="EntryWord"
																   office:value-type="string"/>
											</xsl:otherwise>
										</xsl:choose>
									</text:p>
								</draw:text-box>
							</draw:frame>
						</text:p>
					</style:footer>
				</style:master-page>
			</office:master-styles>
		</office:document-styles>
	</xsl:template>
	<xsl:template name="text-style">
		<xsl:param name="lift-element">Standard</xsl:param>
		<xsl:for-each select="/root/ldml">
			<xsl:variable name="id"
						  select="identity/language/@type"/>
			<xsl:variable name="font"
						  select="special/palaso:defaultFontFamily/@value"/>
			<style:style style:name="{concat($lift-element,'_',$id)}"
						 style:display-name="{concat($lift-element,' ',$id)}"
						 style:family="text"
						 style:parent-style-name="{$lift-element}">
				<style:text-properties style:font-name="{$font}"
									   style:font-name-asian="{$font}"
									   style:font-name-complex="{$font}"/>
			</style:style>
			<xsl:text>
			 </xsl:text>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>
