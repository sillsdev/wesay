<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!-- don't do anything to other versions -->
  <xsl:template match="configuration[@version='8']">
	<configuration version="9">
	  <xsl:apply-templates mode ="Migrate8To9"/>
	</configuration>
  </xsl:template>

  <xsl:template match="@*|node()" mode="Migrate8To9">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="Migrate8To9"/>
	</xsl:copy>
  </xsl:template>

  <xsl:template match="@*|node()" mode="identity">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="identity"/>
	</xsl:copy>
  </xsl:template>

  <!-- ============= actual changing starts here ================ -->
  <xsl:template match="className[text() = 'WeSayDataObject']"   mode="Migrate8To9">
	<className>PalasoDataObject</className>
  </xsl:template>

</xsl:stylesheet>