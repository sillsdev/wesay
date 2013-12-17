<?xml version="1.0"?>
<xsl:transform version="1.0"
				xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
				xmlns:ms="http://schemas.microsoft.com/developer/msbuild/2003"
				xmlns="http://schemas.microsoft.com/developer/msbuild/2003"
				exclude-result-prefixes="ms">

  <xsl:output encoding="ascii" method="xml" omit-xml-declaration="yes" standalone="yes" indent="yes"/>

  <!-- identity transform -->
  <xsl:template match="@*|node()">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"/>
	</xsl:copy>
  </xsl:template>

  <xsl:template match="/">
	<xsl:apply-templates select="@*|node()"/>
  </xsl:template>

  <!-- there is a bug where even when you provide an empty root tag, it is
  still being set to Root Tag. Let's get rid of it. -->
  <xsl:template match="/mergedroot">
	<xsl:apply-templates select="ms:Project"/>
  </xsl:template>

  <!-- make sure we include the update version targets-->
  <xsl:template match="/ms:Project | mergedroot/ms:Project">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()[not(self::ms:Import[contains(@Project, '\bld\Version.Targets')])]"/>
		<Import>
		  <xsl:attribute name="Project">
			<xsl:value-of select="/mergedroot/@projectDirectory"/>
			<xsl:text>\bld\Version.Targets</xsl:text>
		  </xsl:attribute>
		</Import>
	</xsl:copy>
  </xsl:template>

  <!-- make sure out output paths point to the right location-->
  <xsl:template match="ms:OutputPath">
	<xsl:copy>
	  <xsl:choose>
		<xsl:when test="parent::ms:PropertyGroup[contains(@Condition, 'Debug|AnyCPU')]">
		  <xsl:apply-templates select="@*"/>
		  <xsl:value-of select="/mergedroot/@projectDirectory"/>
		  <xsl:text>\output\Debug\</xsl:text>
		</xsl:when>
		<xsl:when test="parent::ms:PropertyGroup[contains(@Condition, 'Release|AnyCPU')]">
		  <xsl:apply-templates select="@*"/>
		  <xsl:value-of select="/mergedroot/@projectDirectory"/>
		  <xsl:text>\output\Release\</xsl:text>
		</xsl:when>
		<xsl:otherwise>
		  <xsl:apply-templates select="@*|node()"/>
		</xsl:otherwise>
	  </xsl:choose>
	</xsl:copy>
  </xsl:template>

  </xsl:transform>