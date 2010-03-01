<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:osis="http://www.bibletechnologies.net/2003/OSIS/namespace"
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
office:version="1.2"
exclude-result-prefixes="xsl"
>
<xsl:output method="xml" omit-xml-declaration="no" indent="yes" />

<xsl:param name="title"></xsl:param>
<xsl:param name="urlBase"></xsl:param>

<xsl:param name="caseLower">abcdefghijklmnopqrstuvwxyz</xsl:param>
<xsl:param name="caseUpper">ABCDEFGHIJKLMNOPQRSTUVWXYZ</xsl:param>
<xsl:param name="baseAlternates">ဣဤဥဦဧဩဪ</xsl:param>
<xsl:param name="baseEquivalent">အအအအအအအ</xsl:param>

<!-- Find the entries before which we need a letter heading
Preparing the variables before hand allows a simple contains test, which is much
faster than using preceding-sibling axis which is extremely slow for large docs.
-->
<xsl:variable name="entries" select="//entry/lexical-unit/form[position()=1]/text" />

<xsl:variable name="firstLetters">
<xsl:for-each select="$entries">
<xsl:value-of select="translate(substring(.,1,1), $baseAlternates, $baseEquivalent)"/>
</xsl:for-each>
</xsl:variable>

<xsl:variable name="firstLetterEntries">
<xsl:for-each select="$entries">
<xsl:variable name="pos" select="position()"/>
<xsl:if test="substring($firstLetters, $pos, 1) != substring($firstLetters, $pos - 1, 1)">
<!--
<xsl:message terminate="no">
	<xsl:value-of select="substring($firstLetters, $pos, 1)"/>
</xsl:message>
-->
<xsl:text>{</xsl:text><xsl:value-of select="ancestor::entry/@id"/><xsl:text>}</xsl:text>
</xsl:if>
</xsl:for-each>
</xsl:variable>


<xsl:template match="/">
<!--
<xsl:message terminate="no"><xsl:value-of select="$firstLetterEntries"/></xsl:message>
-->
<office:document-content office:version="1.2">
<office:scripts>
  <office:event-listeners>
<!--
   <script:event-listener script:language="ooo:script" script:event-name="dom:load" xlink:href="vnd.sun.star.script:WeSay.DictHeaders.js?language=JavaScript&amp;location=document"/>
-->
   <script:event-listener script:language="ooo:script" script:event-name="office:page-count-change" xlink:href="vnd.sun.star.script:WeSay.DictHeaders.js?language=JavaScript&amp;location=document"/>
   <!--
   <script:event-listener script:language="ooo:script" script:event-name="office:print" xlink:href="vnd.sun.star.script:WeSay.DictHeaders.js?language=JavaScript&amp;location=document"/>
   -->
  </office:event-listeners>
 </office:scripts>
<office:font-face-decls/>
<office:automatic-styles>
	<style:style style:name="dictionary" style:family="section">
   <style:section-properties style:editable="false">
	<style:columns fo:column-count="3" fo:column-gap="0.497cm">
	 <style:column style:rel-width="3165*" fo:start-indent="0cm" fo:end-indent="0.249cm"/>
	 <style:column style:rel-width="3307*" fo:start-indent="0.249cm" fo:end-indent="0.249cm"/>
	 <style:column style:rel-width="3166*" fo:start-indent="0.249cm" fo:end-indent="0cm"/>
	</style:columns>
   </style:section-properties>
   </style:style>
</office:automatic-styles>
<office:body>
<office:text text:use-soft-page-breaks="true">
<text:variable-decls>
<text:variable-decl office:value-type="string" text:name="DictFirstWordOnPage"/>
<text:variable-decl office:value-type="string" text:name="DictLastWordOnPage"/>
</text:variable-decls>
<text:sequence-decls>
</text:sequence-decls>
<text:h text:style-name="Title"><xsl:value-of select="$title" /></text:h>
<text:section text:style-name="dictionary" text:name="Dictionary">
   <xsl:apply-templates/>
</text:section>
<text:p text:style-name="Text_20_body"/>
</office:text>
</office:body>
</office:document-content>
</xsl:template>

<xsl:template match="entry">
<xsl:if test="not(@dateDeleted)">
<xsl:if test="contains($firstLetterEntries, concat('{',@id,'}'))">
<!-- Assume that the font for the language headings has been set correctly in styles.xml -->
<text:h text:style-name="Heading_20_1" text:outline-level="1">
<xsl:value-of select="translate(substring(lexical-unit/form/text, 1, 1), $caseLower, $caseUpper)"/>
</text:h>
</xsl:if>

<text:p text:style-name="entry">
<xsl:apply-templates/>
</text:p>
</xsl:if>
</xsl:template>

<xsl:template match="lexical-unit">
<xsl:apply-templates/>
</xsl:template>

<xsl:template match="citation">
<xsl:apply-templates/>
</xsl:template>

<xsl:template match="sense">
<xsl:if test="count(ancestor::entry//sense) &gt; 1">
<xsl:variable name="senseNum"><xsl:number level="multiple" from="entry" count="sense"/></xsl:variable>
<xsl:choose>
<xsl:when test="$senseNum = 1">
<text:variable-set text:name="sense" text:formula="1" office:value-type="float" office:value="{$senseNum}" style:data-style-name="N0"><xsl:value-of select="$senseNum"/></text:variable-set>
</xsl:when>
<xsl:otherwise>
<text:variable-set text:name="sense" text:formula="ooow:sense+1" office:value-type="float" office:value="{$senseNum}" style:data-style-name="N0"><xsl:value-of select="$senseNum"/></text:variable-set>
</xsl:otherwise>
</xsl:choose>
</xsl:if>
<xsl:apply-templates/>
</xsl:template>

<xsl:template match="definition">
<xsl:apply-templates/>
</xsl:template>

<xsl:template match="example">
<xsl:apply-templates/>
</xsl:template>

<xsl:template match="translation">
<xsl:apply-templates/>
</xsl:template>

<xsl:template match="grammatical-info">
</xsl:template>

<xsl:template match="form">
<xsl:apply-templates/><xsl:text> </xsl:text>
</xsl:template>

<xsl:template match="gloss">
<xsl:apply-templates/><xsl:text> </xsl:text>
</xsl:template>

<xsl:template match="text">
<!-- take language from the parent form -->
<xsl:variable name="lang" select="../@lang"/>
<!-- style is based on the element name above the form -->
<xsl:choose>
<xsl:when test="string(local-name(..)) = string('gloss')">
<text:span text:style-name="{concat('gloss_', $lang)}">
<xsl:apply-templates/>
</text:span>
</xsl:when>
<xsl:otherwise>
<xsl:variable name="type" select="local-name(../..)"/>
<text:span text:style-name="{concat($type, '_', $lang)}">
<xsl:apply-templates/>
</text:span>
</xsl:otherwise>
</xsl:choose>
</xsl:template>

<xsl:template match="field"></xsl:template>

<xsl:template match="span">
<xsl:message terminate="no">Span element attributes are currently ignored. They will need to be flattened in the ODF if support is required.</xsl:message>
<xsl:apply-templates />
</xsl:template>

<xsl:template match="illustration">
<!-- really we need to scale these images, but that might be better done in a macro -->
<xsl:choose>
<xsl:when test="substring(@href, 1,1) = '/'">
<draw:frame draw:style-name="fr2" draw:name="{concat('illustration', ancestor::entry/@id)}" text:anchor-type="paragraph" draw:z-index="1"><draw:image xlink:href="{@href}" xlink:type="simple" xlink:show="embed" xlink:actuate="onLoad" draw:filter-name="&lt;All formats&gt;"/></draw:frame>
</xsl:when>
<xsl:when test="contains(@href, ':')">
<draw:frame draw:style-name="fr2" draw:name="{concat('illustration', ancestor::entry/@id)}" text:anchor-type="paragraph" draw:z-index="1"><draw:image xlink:href="{@href}" xlink:type="simple" xlink:show="embed" xlink:actuate="onLoad" draw:filter-name="&lt;All formats&gt;"/></draw:frame>
</xsl:when>
<xsl:otherwise>
<draw:frame draw:style-name="Illustration" draw:name="{concat('illustration_', ancestor::entry/@id)}" text:anchor-type="paragraph" draw:z-index="1" ><draw:image xlink:href="{concat($urlBase,@href)}" xlink:type="simple" xlink:show="embed" xlink:actuate="onLoad" draw:filter-name="&lt;All formats&gt;"/></draw:frame>
</xsl:otherwise>
</xsl:choose>
</xsl:template>

</xsl:stylesheet>
