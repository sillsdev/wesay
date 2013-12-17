<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:output indent="yes"/>

	<xsl:template match="@* | node()">
		<xsl:copy>
			<xsl:apply-templates select="@* | node()"/>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration[@version='2']">
		<xsl:copy>
			<xsl:attribute name="version">3</xsl:attribute>
			<xsl:apply-templates select="node()"/>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="configuration/tasks/task/filter">
		<xsl:copy-of select="field"/>
	</xsl:template>

	<xsl:template match="configuration/tasks/task[@id='Actions']">
		<xsl:copy>
			<xsl:copy-of select="@*"/>
			<xsl:element name="entries">
				<xsl:attribute name="ref">All Entries</xsl:attribute>
			</xsl:element>
			<xsl:apply-templates select="node()"/>
		</xsl:copy>
	</xsl:template>

</xsl:stylesheet>