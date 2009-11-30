<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <!-- don't do anything to other versions -->
  <xsl:template match="configuration[@version='6']">
	<configuration version="7">
	  <xsl:apply-templates mode ="Migrate6To7"/>
	</configuration>
  </xsl:template>

  <xsl:template match="@*|node()" mode="Migrate6To7">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="Migrate6To7"/>
	</xsl:copy>
  </xsl:template>

  <xsl:template match="@*|node()" mode="identity">
	<xsl:copy>
	  <xsl:apply-templates select="@*|node()"  mode="identity"/>
	</xsl:copy>
  </xsl:template>

  <!-- ============= actual changing starts here ================ -->
  <xsl:template match="className[text() = 'WeSayDataObject']"   mode="Migrate6To7">
	<className>PalasoDataObject</className>
  </xsl:template>

</xsl:stylesheet>