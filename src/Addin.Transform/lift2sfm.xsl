<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE xsl:stylesheet [
<!ENTITY nl "
">
]>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:output method="text" omit-xml-declaration="yes"/>
   <xsl:template match="*"/>

   <xsl:template match="text()">
	   <xsl:if test="not(normalize-space() = '')">
		   <xsl:value-of select="."/>
	   </xsl:if>
   </xsl:template>

	<xsl:template match="/">
		<xsl:apply-templates select="//entry"/>
	</xsl:template>

	<xsl:template  match="entry">

		<xsl:apply-templates />
		<xsl:if test="@dateModified">
			<xsl:text>&nl;\dt </xsl:text><xsl:value-of select="substring(@dateModified, 1, 10)"/>
		</xsl:if>
		<xsl:text>&nl;</xsl:text>
	</xsl:template>

	<xsl:template match="lexical-unit">
		<xsl:apply-templates>
			<xsl:with-param name="prefix">lx</xsl:with-param>
		</xsl:apply-templates>
	</xsl:template>


	  <xsl:template match="field">
		<xsl:apply-templates select="form">
			<xsl:with-param name="prefix">
				<xsl:value-of select="@tag"/>
			 </xsl:with-param>
		</xsl:apply-templates>
	</xsl:template>

  <xsl:template match="relation">
	<xsl:if test="not(@ref = '')">
	  <xsl:text>&nl;\</xsl:text>
	  <xsl:value-of select="@name"/>
	  <xsl:text>   </xsl:text>
	  <xsl:for-each select="//entry[@id=current()/@ref]/lexical-unit"><!-- todo: what if the ref is not found?-->
		<xsl:apply-templates mode="raw"/>
	  </xsl:for-each>
	</xsl:if>
  </xsl:template>

  <xsl:template match="trait">
		<xsl:text>&nl;\</xsl:text><xsl:value-of select="@name"/>
	  <xsl:text>   </xsl:text>
		<xsl:value-of select="@value"/>
	</xsl:template>

  <!-- <xsl:template match="trait[@name='SemanticDomainDdp4']">
		<xsl:text>&nl;\sd</xsl:text>
	  <xsl:text>   </xsl:text>
		<xsl:value-of select="@value"/>
	</xsl:template>
-->




  <xsl:template match="sense">
	<xsl:if test="grammatical-info/@value">
		  <xsl:text>&nl;\ps </xsl:text><xsl:value-of select="grammatical-info/@value"/>
	</xsl:if>
		<xsl:apply-templates select="gloss">
			<xsl:sort select="@lang"/>
		</xsl:apply-templates>
		 <xsl:apply-templates select="trait"/>
		 <xsl:apply-templates select="definition"/>
		 <xsl:apply-templates select="example"/>
		 <xsl:apply-templates select="field"/>
		 <xsl:apply-templates select="note"/>
		 <xsl:apply-templates select="picture"/>
	 </xsl:template>

  <xsl:template match="definition">
	<xsl:apply-templates select="form">
	  <xsl:with-param name="prefix">d</xsl:with-param>
	</xsl:apply-templates>
	<xsl:apply-templates select="translation"/>
  </xsl:template>

<xsl:template match="example">
	<xsl:apply-templates select="form">
		<xsl:with-param name="prefix">x</xsl:with-param>
	</xsl:apply-templates>
	<xsl:apply-templates select="translation"/>
</xsl:template>

  <xsl:template match="field[@tag='Definition']">
	<!-- this is here as a result of a screwup where definition was treated as property, rather than a proper
	part of lift... projects created after oct 23 2007 won't need this-->
	<xsl:apply-templates select="form">
	  <xsl:with-param name="prefix">d</xsl:with-param>
	</xsl:apply-templates>
  </xsl:template>

	<xsl:template match="field[@tag='Note']">
		<xsl:apply-templates select="form">
			<xsl:with-param name="prefix">nt</xsl:with-param>
		</xsl:apply-templates>
	</xsl:template>

  <xsl:template match="field[@tag='CitationForm']">
	<xsl:apply-templates select="form">
	  <xsl:with-param name="prefix">lc</xsl:with-param>
	</xsl:apply-templates>
  </xsl:template>

  <xsl:template match="field[@tag='LiteralMeaning']">
	<!--review: notice we drop the language identifier completely -->
	<xsl:text>&nl;\lt </xsl:text>
  </xsl:template>

  <!-- Wesay frowns on encouraging people to do homograph numbers, so it's not a build-in field.  As a concession,
  we detect that someone has added it as a custom field if they've named it anything reasonable-->
  <xsl:template match="field[contains(@tag,'omograph')] | field[contains(@tag,'omonym')]  | field[@tag='hm']">
	<!--notice we drop the language identifier completely -->
	<xsl:text>&nl;\hm </xsl:text><xsl:value-of select="form/text"/>
  </xsl:template>

  <!-- TODO: LIFT has a proper place for etymology, this won't address this. -->
  <!-- Wesay doesn't expect its users to doing etymology, so it's not a build-in field.  As a concession,
  we try to detect it and export it correctly -->
  <xsl:template match="field[@tag='Etymology'] |field[@tag='et']">
	<!--notice we drop the language identifier completely -->
	<xsl:text>&nl;\et </xsl:text>
	<xsl:value-of select="form/text"/>
  </xsl:template>

  <!-- Wesay doesn't expect its users to doing etymology, so it's not a build-in field.  As a concession,
  we try to detect it and export it correctly -->
  <xsl:template match="field[contains(@tag,'orrowed')] |field[@tag='bw']">
	<!--review: notice we drop the language identifier completely -->
	<xsl:text>&nl;\bw </xsl:text>
	<xsl:value-of select="form/text"/>
  </xsl:template>

  <!-- Wesay doesn't expect its users to doing encyclopedic info, so it's not a build-in field.  As a concession,
  we try to detect it and export it correctly -->
  <xsl:template match="field[@tag='EncyclopedicInformation'] |field[@tag='ee']">
	<!--review: notice we drop the language identifier completely -->
	<xsl:text>&nl;\ee </xsl:text>
	<xsl:value-of select="form/text"/>
  </xsl:template>

  <xsl:template match="translation">
		<xsl:apply-templates select="form">
			<xsl:with-param name="prefix">x</xsl:with-param>
		</xsl:apply-templates>
	 </xsl:template>

  <xsl:template match="picture">
	<xsl:text>&nl;\pc WeSay/pictures/</xsl:text><xsl:value-of select="@href"/>
  </xsl:template>

	<xsl:template match="gloss">
		<xsl:choose>
			<xsl:when test="not(preceding-sibling::gloss[@lang=current()/@lang])"><!-- are we the first gloss with this lang -->
				<xsl:text>&#13;\g_</xsl:text><xsl:value-of select="@lang"/><xsl:text> </xsl:text>
			</xsl:when>
			<xsl:otherwise>; </xsl:otherwise>
		</xsl:choose>
		<xsl:apply-templates select="text"/>
	</xsl:template>

	  <xsl:template match="form">
		  <xsl:param name="prefix"/>

		  <xsl:text>&nl;\</xsl:text><xsl:value-of select="$prefix"/><xsl:text>_</xsl:text>
		<xsl:value-of select="@lang"/><xsl:text>   </xsl:text>
		<xsl:apply-templates select="text" />
	  </xsl:template>

	   <xsl:template match="form" mode="raw">

		<xsl:apply-templates select="text" />
	  </xsl:template>

	<xsl:template match="text">
		<xsl:value-of select="normalize-space()"/>
	</xsl:template>

  <!-- omit empty traits -->
  <xsl:template match="trait[not(@value)]"/>

  <!-- omit empty traits -->
  <xsl:template match="trait[@value = '']"/>

  <xsl:template match="trait[starts-with(@name, 'flag_')]">
	<!-- don't output these-->
  </xsl:template>

  <!-- omit entries that have been deleted-->
  <xsl:template match="entry[@dateDeleted]"/>

</xsl:stylesheet>
